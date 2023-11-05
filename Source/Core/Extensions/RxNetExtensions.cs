using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Proxoft.Redux.Core.Extensions;

public static class RxNetExtensions
{
    public static IObservable<Unit> SelectVoid<TS>(this IObservable<TS> source)
    {
        return source
            .Select(_ => Unit.Default);
    }

    public static IObservable<TR> SelectDistinctUntilChanged<T, TR>(
        this IObservable<T> source,
        Func<T, TR> selector
    )
    {
        return source
            .Select(selector)
            .DistinctUntilChanged();
    }

    public static IObservable<TR> SelectDistinctUntilChanged<T, TR, TKey>(
        this IObservable<T> source,
        Func<T, TR> selector,
        Func<TR, TKey> keySelector
    )
    {
        return source
            .Select(selector)
            .DistinctUntilChanged(keySelector);
    }

    public static IObservable<T> OfTypeWhere<T>(
        this IObservable<T> source,
        Func<T, bool> predicate)
        where T : class
    {
        return source
            .OfType<T>()
            .Where(i => predicate(i));
    }

    public static IObservable<TR> SelectAsync<TS, TR>(
        this IObservable<TS> source,
        Func<TS, Task<TR>> taskFactory)
    {
        return source
            .Select(s => Observable.FromAsync(() => taskFactory(s)))
            .Merge();
    }

    public static IObservable<T> DoAsync<T>(
        this IObservable<T> source,
        Func<T, Task> taskFactory)
    {
        return source
            .SelectAsync(s => ExecuteTask(s, taskFactory));
    }

    public static void OnNextWhenChanged<T>(this BehaviorSubject<IReadOnlyCollection<T>> observable, IReadOnlyCollection<T> newValue)
    {
        if (observable.Value.SequenceEqual(newValue))
        {
            return;
        }

        observable.OnNext(newValue);
    }

    private static async Task<T> ExecuteTask<T>(T throughput, Func<T, Task> taskToExecute)
    {
        await taskToExecute(throughput);
        return throughput;
    }
}
