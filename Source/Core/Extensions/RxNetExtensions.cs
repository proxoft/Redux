using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Proxoft.Redux.Core.Extensions
{
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
    }
}
