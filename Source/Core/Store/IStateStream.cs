using System;

namespace Proxoft.Redux.Core
{
    public interface IStateStream<T> : IObservable<T>
    {
    }
}
