using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Guards;

namespace Proxoft.Redux.Hosting.Builders;

internal sealed class FuncGuard<TState> : IGuard<TState>
{
    private readonly Func<IAction, TState, IAction> _guardFunc;

    public FuncGuard(Func<IAction, TState, IAction> guardFunc)
    {
        _guardFunc = guardFunc;
    }

    public IAction Validate(IAction action, TState state)
    {
        return _guardFunc(action, state);
    }
}
