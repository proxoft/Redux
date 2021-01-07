using System;
using Proxoft.Redux.Core;

namespace Proxoft.Redux.Hosting.Builders
{
    internal class FuncReducer<TState> : IReducer<TState>
    {
        private readonly Func<TState, IAction, TState> _func;

        public FuncReducer(Func<TState, IAction, TState> func)
        {
            _func = func;
        }

        public TState Reduce(TState state, IAction action)
            => _func(state, action);
    }
}
