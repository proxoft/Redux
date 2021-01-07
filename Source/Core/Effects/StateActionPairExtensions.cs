using System;
using System.Reactive.Linq;

namespace Proxoft.Redux.Core
{
    public static class StateActionPairExtensions
    {
        public static IObservable<StateActionPair<TState>> WhereAction<TState>(this IObservable<StateActionPair<TState>> source, Func<IAction, bool> predicate)
            => source.Where(pair => predicate(pair.Action));
    }
}
