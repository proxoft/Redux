using Proxoft.Redux.Core;

namespace BlazorApp.Client.Application.Counters
{
    public static class CounterReducer
    {
        public static CounterState Reduce(CounterState state, IAction action)
        {
            return action switch
            {
                IncreaseCounterAction ia => state with { Value = state.Value + ia.ByValue },
                ResetCounterAction _ => new (),
                _ => state
            };
        }
    }
}
