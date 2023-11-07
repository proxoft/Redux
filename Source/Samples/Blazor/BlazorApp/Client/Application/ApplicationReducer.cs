using Proxoft.Redux.BlazorApp.Client.Application.Counters;
using Proxoft.Redux.BlazorApp.Client.Application.Forecasts;
using Proxoft.Redux.Core;

namespace Proxoft.Redux.BlazorApp.Client.Application;

public class ApplicationReducer : IReducer<ApplicationState>
{
    public ApplicationState Reduce(ApplicationState state, IAction action)
    {
        var counter = CounterReducer.Reduce(state.Counter, action);
        var forecast = ForecastReducer.Reduce(state.Forecast, action);

        return state with {
            Counter = counter,
            Forecast = forecast
        };
    }
}
