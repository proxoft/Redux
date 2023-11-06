using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Proxoft.Redux.Core.Actions;
using Proxoft.Redux.Core.Dispatchers;
using Proxoft.Redux.Core.ExceptionHandling;
using Proxoft.Redux.Core.Guards;

namespace Proxoft.Redux.Core;

public sealed class Store<T>(
    IActionDispatcher dispatcher,
    IReducer<T> reducer,
    IActionGuard<T> actionGuard,
    IStateStreamSubject<T> stateStreamSubject,
    IEnumerable<IEffect<T>> effects,
    IExceptionHandler exceptionHandler,
    ILogger<Store<T>> logger) : IDisposable
{
    private readonly Subject<StateActionPair<T>> _effectStream = new();

    private readonly IActionDispatcher _dispatcher = dispatcher;
    private readonly IReducer<T> _reducer = reducer;
    private readonly IStateStreamSubject<T> _stateStreamSubject = stateStreamSubject;
    private readonly IEnumerable<IEffect<T>> _effects = effects.ToArray();
    private readonly IActionGuard<T> _actionGuard = actionGuard;
    private readonly IExceptionHandler _exceptionHandler = exceptionHandler;
    private readonly ILogger<Store<T>> _logger = logger;

    private IDisposable? _dispatcherSubscription;
    private IDisposable? _effectsSubscription;

    public IActionDispatcher Dispatcher => _dispatcher;

    public IStateStream<T> StateStream => _stateStreamSubject;

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
                    IAction guardedAction = _actionGuard.Validate(action, acc.State);
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

public static class StoreHelper
{
    public static Store<TState> Create<TState>(
        IReducer<TState> reducer,
        IExceptionHandler? exceptionHandler = null,
        ILogger<Store<TState>>? logger = null,
        params IEffect<TState>[] effects)
    {
        return Create(
            reducer,
            exceptionHandler: exceptionHandler,
            logger: logger,
            effects: effects
        );
    }

    public static Store<TState> Create<TState>(
        Func<TState, IAction, TState> reducer,
        IExceptionHandler? exceptionHandler = null,
        ILogger<Store<TState>>? logger = null,
        params IEffect<TState>[] effects)
    {
        return Create(
            new FuncReducer<TState>(reducer),
            exceptionHandler: exceptionHandler,
            logger: logger,
            effects: effects);
    }

    public static Store<TState> Create<TState>(
        IReducer<TState> reducer,
        Func<IAction, TState, IAction> actionGuard,
        IExceptionHandler? exceptionHandler = null,
        ILogger<Store<TState>>? logger = null,
        params IEffect<TState>[] effects)
    {
        return Create(
            reducer,
            actionGuard: new FuncGuard<TState>(actionGuard),
            exceptionHandler: exceptionHandler,
            logger: logger,
            effects: effects);
    }

    public static Store<TState> Create<TState>(
        Func<TState, IAction, TState> reducer,
        Func<IAction, TState, IAction> actionGuard,
        IExceptionHandler? exceptionHandler = null,
        ILogger<Store<TState>>? logger = null,
        params IEffect<TState>[] effects)
    {
        return Create(
            new FuncReducer<TState>(reducer),
            actionGuard: new FuncGuard<TState>(actionGuard),
            exceptionHandler: exceptionHandler,
            logger: logger,
            effects: effects);
    }

    public static Store<TState> Create<TState>(
        IReducer<TState> reducer,
        IActionGuard<TState>? actionGuard = null,
        IExceptionHandler? exceptionHandler = null,
        ILogger<Store<TState>>? logger = null,
        params IEffect<TState>[] effects)
    {
        DefaultActionDispatcher dispatcher = DefaultActionDispatcher.Create();
        DefaultStateStream<TState> stateStreamSubject = new();

        return new Store<TState>(
            dispatcher,
            reducer,
            actionGuard ?? new NoGuard<TState>(),
            stateStreamSubject,
            effects,
            exceptionHandler ?? new ActionExceptionHandler(ex => throw ex),
            logger ?? NullLogger<Store<TState>>.Instance
        );
    }
}