namespace Proxoft.Redux.Core.Guards;

public sealed class NoGuard<T> : IGuard<T>
{
    public IAction Validate(IAction action, T state)
    {
        return action;
    }
}
