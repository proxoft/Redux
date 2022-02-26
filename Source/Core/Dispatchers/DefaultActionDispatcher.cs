using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Proxoft.Redux.Core.Dispatchers
{
    public class DefaultActionDispatcher : IActionDispatcher
    {
        private readonly Subject<IAction> _dispatcher;
        private readonly IObservable<IAction> _observableDispatcher;

        public DefaultActionDispatcher(
            IActionJournaler actionJournaler,
            IScheduler scheduler)
        {
            _dispatcher = new Subject<IAction>();
            _observableDispatcher = _dispatcher
                .Do(action => actionJournaler.Journal(action))
                .ObserveOn(scheduler);
        }

        public void Dispatch(IAction action)
        {
            _dispatcher.OnNext(action);
        }

        public IDisposable Subscribe(IObserver<IAction> observer)
            => _observableDispatcher.Subscribe(observer);
    }
}
