namespace Proxoft.Redux.Core
{
    public interface IReducer<T>
    {
        T Reduce(T state, IAction action);
    }
}
