using System;

namespace Proxoft.Redux.Core
{
    public interface IEffect<TState> : IDisposable
    {
        IObservable<IAction> OutActions { get; }

        void Connect(IObservable<StateActionPair<TState>> stateActionStream);

        void Disconnect();
    }
}
