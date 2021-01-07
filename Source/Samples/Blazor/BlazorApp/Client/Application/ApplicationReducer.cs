using BlazorApp.Client.Application.Counters;
using BlazorApp.Client.Application.Forecasts;
using Proxoft.Redux.Core;

namespace BlazorApp.Client.Application
{
    public class ApplicationReducer : IReducer<ApplicationState>
    {
        public ApplicationState Reduce(ApplicationState state, IAction action)
        {
            var counter = CounterReducer.Reduce(state.Counter, action);
            var forecast = ForecastReducer.Reduce(state.Forecast, action);

            if (counter == state.Counter && forecast == state.Forecast)
            {
                return state;
            }

            return state with {
                Counter = counter,
                Forecast = forecast
            };
        }
    }
}
