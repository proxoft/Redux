namespace Proxoft.Redux.Core.Guards;

public sealed class NoGuard<T> : IActionGuard<T>
{
    public IAction Validate(IAction action, T state)
    {
        return action;
    }
}
