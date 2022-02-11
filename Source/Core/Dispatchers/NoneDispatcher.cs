using System;
using System.Reactive.Linq;

namespace Proxoft.Redux.Core.Dispatchers
{
    public class NoneDispatcher : IActionDispatcher
    {
        public static NoneDispatcher Instance = new NoneDispatcher();

        private IObservable<IAction> _never = Observable.Never<IAction>();

        private NoneDispatcher()
        {
        }

        public void Dispatch(IAction action)
        {
        }

        public IDisposable Subscribe(IObserver<IAction> observer)
        {
            return _never.Subscribe(observer);
        }
    }
}
