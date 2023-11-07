using System;

namespace Proxoft.Redux.Core;

public class FuncReducer<TState>(Func<TState, IAction, TState> func) : IReducer<TState>
{
    private readonly Func<TState, IAction, TState> _func = func;

    public TState Reduce(TState state, IAction action)
        => _func(state, action);
}
