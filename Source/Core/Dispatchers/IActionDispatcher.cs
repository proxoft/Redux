using System;

namespace Proxoft.Redux.Core
{
    public interface IActionDispatcher : IObservable<IAction>
    {
        void Dispatch(IAction action);
    }
}
