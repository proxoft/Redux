using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Proxoft.Redux.Core;

public class DefaultStateStream<T> : IStateStreamSubject<T>
{
    private readonly BehaviorSubject<T> _subject;
    private readonly IObservable<T> _observable;

    public DefaultStateStream()
    {
        _subject = new BehaviorSubject<T>(default);
        _observable = _subject;
    }

    public DefaultStateStream(IScheduler scheduler) : this()
    {
        _observable = _subject.ObserveOn(scheduler);
    }

    public void OnNext(T state)
        => _subject.OnNext(state);

    public IDisposable Subscribe(IObserver<T> observer)
        => _observable.Subscribe(observer);
}
