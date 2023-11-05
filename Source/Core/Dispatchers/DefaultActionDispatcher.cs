using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Proxoft.Redux.Core.Dispatchers;

public class DefaultActionDispatcher : IActionDispatcher
{
    private readonly Subject<(Type?, IAction)> _dispatcher;
    private readonly IObservable<IAction> _observableDispatcher;

    public DefaultActionDispatcher(
        IActionJournaler actionJournaler,
        IScheduler scheduler)
    {
        _dispatcher = new Subject<(Type?, IAction)>();
        _observableDispatcher = _dispatcher
            .Do(action => actionJournaler.Journal(action.Item2, action.Item1))
            .Select(x => x.Item2)
            .ObserveOn(scheduler);
    }

    public void Dispatch(IAction action, Type? sender = null)
    {
        _dispatcher.OnNext((sender, action));
    }

    public IDisposable Subscribe(IObserver<IAction> observer)
        => _observableDispatcher.Subscribe(observer);
}
