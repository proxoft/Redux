using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Proxoft.Redux.Core.Actions;
using Proxoft.Redux.Core.ExceptionHandling;

namespace Proxoft.Redux.Core
{
    public sealed class Store<T>: IDisposable
    {
        private readonly IActionDispatcher _dispatcher;
        private readonly IReducer<T> _reducer;
        private readonly IStateStreamSubject<T> _stateStreamSubject;
        private readonly IEnumerable<IEffect<T>> _effects;
        private readonly Subject<StateActionPair<T>> _effectStream = new();
        private readonly IExceptionHandler _exceptionHandler;

        private IDisposable _dispatcherSubscription;
        private IDisposable _effectsSubscription;

        public Store(
            IActionDispatcher dispatcher,
            IReducer<T> reducer,
            IStateStreamSubject<T> stateStreamSubject,
            IEnumerable<IEffect<T>> effects,
            IExceptionHandler exceptionHandler)
        {
            _dispatcher = dispatcher;
            _reducer = reducer;
            _stateStreamSubject = stateStreamSubject;
            _effects = effects;
            _exceptionHandler = exceptionHandler;
        }

        public void Dispose()
        {
            foreach(var e in _effects)
            {
                e.Disconnect();
            }

            _effectsSubscription?.Dispose();
            _effectsSubscription = null;

            _dispatcherSubscription?.Dispose();
            _dispatcherSubscription = null;
        }

        public void Intialize(T initialState)
            => this.Initialize(() => initialState);

        public void Initialize(Func<T> initialState)
        {
            var init = initialState();

            _stateStreamSubject.OnNext(init);

            _dispatcherSubscription = _dispatcher
                .Scan(
                    new StateActionPair<T>(init, null),
                    (acc, action) =>
                    {
                        var state = _reducer.Reduce(acc.State, action);
                        return new(state, action);
                    })
                .Do(pair =>
                {
                    _stateStreamSubject.OnNext(pair.State);
                    _effectStream.OnNext(pair);
                })
                .Subscribe(
                    pair =>
                    {
                        // eventually do (debug) log
                    },
                    e => _exceptionHandler.OnException(e)
                );

            _dispatcher.Dispatch(DefaultActions.Initialize);

            foreach (var e in _effects.OfType<IEffect<T>>())
            {
                e.Connect(_effectStream);
            }

            _effectsSubscription = Observable
                .Merge(_effects.Select(e => e.Actions))
                .Subscribe(a => _dispatcher.Dispatch(a));

            _dispatcher.Dispatch(DefaultActions.InitializeEffects);
        }
    }
}
