using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Proxoft.Redux.Core.Dispatcher
{
    public class DefaultActionDispatcher : IActionDispatcher
    {
        private readonly Subject<IAction> _dispatcher;
        private readonly IObservable<IAction> _observableDispatcher;

        public DefaultActionDispatcher() : this(Scheduler.CurrentThread)
        {
        }

        public DefaultActionDispatcher(IScheduler scheduler)
        {
            _dispatcher = new();
            _observableDispatcher = _dispatcher.ObserveOn(scheduler);
        }

        public void Dispatch(IAction action)
        {
            _dispatcher.OnNext(action);
        }

        public IDisposable Subscribe(IObserver<IAction> observer)
            => _observableDispatcher.Subscribe(observer);
    }
}
