using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Guards;

namespace Proxoft.Redux.Core.Guards;

public sealed class FuncGuard<TState>(Func<IAction, TState, IAction> guardFunc) : IGuard<TState>
{
    private readonly Func<IAction, TState, IAction> _guardFunc = guardFunc;

    public IAction Validate(IAction action, TState state)
    {
        return _guardFunc(action, state);
    }
}
