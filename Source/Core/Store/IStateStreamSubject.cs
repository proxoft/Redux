namespace Proxoft.Redux.Core
{
    public interface IStateStreamSubject<T> : IStateStream<T>
    {
        void OnNext(T state);
    }
}
