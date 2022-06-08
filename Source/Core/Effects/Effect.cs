using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Proxoft.Redux.Core.Effects;
using Proxoft.Redux.Core.ExceptionHandling;
using Proxoft.Redux.Core.Tools;

namespace Proxoft.Redux.Core
{
    public abstract class Effect<TState> : IEffect<TState>
    {
        private readonly SubscriptionsManager _subscriptionsManager = new SubscriptionsManager();
        private readonly Subject<IAction> _actionsSubject = new Subject<IAction>();

        public IObservable<IAction> OutActions => _actionsSubject;

        protected IObservable<StateActionPair<TState>> StateActionStream { get; private set; } = Observable.Never<StateActionPair<TState>>();

        protected IObservable<IAction> ActionStream => this.StateActionStream.Select(pair => pair.Action);

        protected IObservable<TState> StateStream => this.StateActionStream.Select(pair => pair.State);

        public void Connect(IObservable<StateActionPair<TState>> stateActionStream)
        {
            this.StateActionStream = stateActionStream;

            var subscriptions = this.AutoSubscribe()
                .Concat(this.OnConnect())
                .ToArray();

            this.AddSubscriptions(subscriptions);
        }

        public void Disconnect()
        {
            _subscriptionsManager.RemoveSubscriptions();
            this.OnDisconnect();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispatch(IAction action)
            => _actionsSubject.OnNext(action);

        protected virtual IEnumerable<IDisposable> OnConnect()
        {
            return Array.Empty<IDisposable>();
        }

        protected virtual void OnDisconnect()
        {
        }

        /// <summary>
        /// Creates a subscription which dispatches the action(s) for all provided action streams.
        /// </summary>
        /// <param name="actionStreams">action streams</param>
        /// <returns>Subscription instance.</returns>
        [Obsolete("Use the overload with Subscription parameter instead, it provides better exception handling")]
        protected IDisposable SubscribeDispatch(params IObservable<IAction>[] actionStreams)
        {
            var subscription = actionStreams
                .Merge()
                .Subscribe(this.Dispatch);

            return subscription;
        }

        protected IEnumerable<IDisposable> SubscribeDispatch(params Subscription<IAction>[] actionStreams)
        {
            var subscription = actionStreams
                .Select(x => x.Observable.Subscribe(
                    this.Dispatch,
                    exception => throw new ReduxException(x, exception))
                );

            return subscription;
        }

        /// <summary>
        /// Creates a subscription which dispatches the action(s) for all provided action streams.
        /// </summary>
        /// <param name="actionStreams">action streams</param>
        /// <returns>Subscription instance.</returns>
        [Obsolete("Use the overload with Subscription parameter instead, it provides better exception handling")]
        protected IDisposable SubscribeDispatch(params IObservable<IAction[]>[] actionStreams)
        {
            var subscriptions = actionStreams
                .Merge()
                .Subscribe(actions =>
                {
                    foreach (var action in actions)
                    {
                        this.Dispatch(action);
                    }
                });

            return subscriptions;
        }

        protected IEnumerable<IDisposable> SubscribeDispatch(params Subscription<IAction[]>[] actionStreams)
        {
            var subscriptions = actionStreams
                .Select(x => x.Observable.Subscribe(
                    actions =>
                    {
                        foreach (var action in actions)
                        {
                            this.Dispatch(action);
                        }
                    },
                    exception => throw new ReduxException(x, exception))
                );

            return subscriptions;
        }

        /// <summary>
        /// Creates a subscription for all observables which don't dispatch any action
        /// </summary>
        /// <param name="sources">Streams.</param>
        /// <returns>Subscription instance.</returns>
        [Obsolete("Use the overload with Subscription parameter instead, it provides better exception handling")]
        protected IDisposable SubscribeNoDispatch(params IObservable<Unit>[] sources)
        {
            var subscription = sources
                .Merge()
                .Subscribe();

            return subscription;
        }

        protected IEnumerable<IDisposable> SubscribeNoDispatch(params Subscription<Unit>[] sources)
        {
            var subscription = sources
                .Select(x => x.Observable.Subscribe(
                    unit => { },
                    exception => throw new ReduxException(x, exception))
                );

            return subscription;
        }

        protected void AddSubscriptions(params IDisposable[] subscriptions)
            => _subscriptionsManager.AddSubscriptions(subscriptions);

        public void RemoveSubscriptions(params IDisposable[] subscriptions)
            => _subscriptionsManager.RemoveSubscriptions(subscriptions);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _subscriptionsManager.Dispose();
            _actionsSubject.Dispose();
        }

        private IDisposable[] AutoSubscribe()
        {
            var behavior = this.GetType().GetCustomAttribute<AutoSubscribeAttribute>() ?? new AutoSubscribeAttribute();

            return this.SubscribeProperties(!behavior.Properties)
                .Union(this.SubscribeMethods(!behavior.Methods))
                .ToArray();
        }

        private IEnumerable<IDisposable> SubscribeProperties(bool optIn)
        {
            var voids = this.GetObservableProperties<Unit>(optIn).ToArray();
            var actions = this.GetObservableProperties<IAction>(optIn).ToArray();
            var arrayActions = this.GetObservableProperties<IAction[]>(optIn).ToArray();

            return this.SubscribeNoDispatch(voids)
                .Union(this.SubscribeDispatch(actions))
                .Union(this.SubscribeDispatch(arrayActions));
        }

        private IEnumerable<IDisposable> SubscribeMethods(bool optIn)
        {
            var voids = this.GetObservableMethods<Unit>(optIn).ToArray();
            var actions = this.GetObservableMethods<IAction>(optIn).ToArray();
            var arrayActions = this.GetObservableMethods<IAction[]>(optIn).ToArray();

            return this.SubscribeNoDispatch(voids)
                .Union(this.SubscribeDispatch(actions))
                .Union(this.SubscribeDispatch(arrayActions));
        }
    }
}
