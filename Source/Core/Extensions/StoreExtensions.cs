using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Proxoft.Redux.Core.Extensions
{
    public static class StoreExtensions
    {
        public static IObservable<(TS, TA)> WhereStateAction<TS, TA>(
            this IObservable<StateActionPair<TS>> source,
            Func<TS, bool> statePredicate)
            where TA : IAction
        {
            return source.
                WhereStateAction<TS, TA>(statePredicate, _ => true);
        }

        public static IObservable<(TS, TA)> WhereStateAction<TS, TA>(
            this IObservable<StateActionPair<TS>> source,
            Func<TA, bool> actionPredicate)
            where TA : IAction
        {
            return source.
                WhereStateAction(_ => true, actionPredicate);
        }

        public static IObservable<(TS, TA)> WhereStateAction<TS, TA>(
            this IObservable<StateActionPair<TS>> source,
            Func<TS, bool> statePredicate,
            Func<TA, bool> actionPredicate)
            where  TA : IAction
        {
            return source
                .Where(pair => (pair.Action is TA taAction)
                            && actionPredicate(taAction)
                            && statePredicate(pair.State))
                .Select(pair => (pair.State, (TA)pair.Action));
        }

        public static IObservable<TS> SelectState<TS>(this IObservable<StateActionPair<TS>> source)
        {
            return source
                .Select(pair => pair.State);
        }

        public static IObservable<IAction> SelectAction<TS>(this IObservable<StateActionPair<TS>> source)
        {
            return source
                .Select(pair => pair.Action);
        }

        public static IObservable<TS> SelectState<TS, TA>(this IObservable<(TS state, TA action)> source)
            where TA : IAction
        {
            return source
                .Select(pair => pair.state);
        }

        public static IObservable<TA> SelectAction<TS, TA>(this IObservable<(TS state, TA action)> source)
            where TA : IAction
        {
            return source
                .Select(pair => pair.action);
        }
    }
}
