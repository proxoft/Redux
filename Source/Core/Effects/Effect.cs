﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            var subscriptions = this.OnConnect().ToArray();
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

        protected abstract IEnumerable<IDisposable> OnConnect();

        protected virtual void OnDisconnect()
        {
        }

        /// <summary>
        /// Creates a subscription which dispatches the action(s) for all provided action streams.
        /// </summary>
        /// <param name="actionStreams">action streams</param>
        /// <returns>Subscription instance.</returns>
        protected IDisposable SubscribeDispatch(params IObservable<IAction>[] actionStreams)
        {
            var subscription = actionStreams
                .Merge()
                .Subscribe(this.Dispatch);

            return subscription;
        }

        /// <summary>
        /// Creates a subscription which dispatches the action(s) for all provided action streams.
        /// </summary>
        /// <param name="actionStreams">action streams</param>
        /// <returns>Subscription instance.</returns>
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

        /// <summary>
        /// Creates a subscription for all observables which don't dispatch any action
        /// </summary>
        /// <param name="sources">Streams.</param>
        /// <returns>Subscription instance.</returns>
        protected IDisposable SubscribeNoDispatch(params IObservable<Unit>[] sources)
        {
            var subscription = sources
                .Merge()
                .Subscribe();

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
    }
}
