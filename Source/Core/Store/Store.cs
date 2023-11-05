using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Proxoft.Redux.Core.Actions;
using Proxoft.Redux.Core.ExceptionHandling;
using Proxoft.Redux.Core.Guards;

namespace Proxoft.Redux.Core;

public sealed class Store<T>: IDisposable
{
    private readonly Subject<StateActionPair<T>> _effectStream = new();

    private readonly IActionDispatcher _dispatcher;
    private readonly IReducer<T> _reducer;
    private readonly IStateStreamSubject<T> _stateStreamSubject;
    private readonly IEnumerable<IEffect<T>> _effects;
    private readonly IGuard<T> _guard;
    private readonly IExceptionHandler _exceptionHandler;
    private readonly ILogger<Store<T>> _logger;

    private IDisposable? _dispatcherSubscription;
    private IDisposable? _effectsSubscription;

    public Store(
        IActionDispatcher dispatcher,
        IReducer<T> reducer,
        IGuard<T> guard,
        IStateStreamSubject<T> stateStreamSubject,
        IEnumerable<IEffect<T>> effects,
        IExceptionHandler exceptionHandler,
        ILogger<Store<T>> logger)
    {
        _dispatcher = dispatcher;
        _reducer = reducer;
        _stateStreamSubject = stateStreamSubject;
        _effects = effects.ToArray();
        _guard = guard;
        _exceptionHandler = exceptionHandler;
        _logger = logger;
    }

    public void Initialize(T initialState)
        => this.Initialize(() => initialState);

    public void Initialize(Func<T> initialState)
    {
        var init = initialState();

        _stateStreamSubject.OnNext(init);

        _dispatcherSubscription = _dispatcher
            .Where(action => action != DefaultActions.None)
            .Scan(
                new StateActionPair<T>(init, DefaultActions.None),
                (acc, action) =>
                {
                    IAction guardedAction = _guard.Validate(action, acc.State);
                    if(guardedAction != action)
                    {
                        _logger.LogDebug($"action {action} changed to {guardedAction}");
                    }

                    if(guardedAction == DefaultActions.None)
                    {
                        return acc;
                    }

                    var state = _reducer.Reduce(acc.State, guardedAction);
                    return new StateActionPair<T>(state, guardedAction);
                })
            .DistinctUntilChanged()
            .Do(pair =>
            {
                _stateStreamSubject.OnNext(pair.State);
                _effectStream.OnNext(pair);
            })
            .Subscribe(
                pair =>
                {
                    _logger.LogTrace($"{pair}");
                },
                e =>
                {
                    _logger.LogError(e, "unexpected error occurred in Store pipeline");
                    _exceptionHandler.OnException(e);
                },
                () =>
                {
                    _logger.LogDebug("store was completed");
                }
            );

        _dispatcher.Dispatch(DefaultActions.Initialize, this.GetType());

        foreach (var e in _effects.OfType<IEffect<T>>())
        {
            e.Connect(_effectStream);
        }

        _effectsSubscription = Observable
            .Merge(_effects.Select(e => e.OutActions.CombineLatest(Observable.Return(e.GetType()))))
            .Subscribe(a => _dispatcher.Dispatch(a.First, a.Second));

        _dispatcher.Dispatch(DefaultActions.InitializeEffects, this.GetType());
    }

    public void Dispose()
    {
        foreach (var e in _effects)
        {
            e.Disconnect();
        }

        _effectsSubscription?.Dispose();
        _effectsSubscription = null;

        _dispatcherSubscription?.Dispose();
        _dispatcherSubscription = null;
    }
}
