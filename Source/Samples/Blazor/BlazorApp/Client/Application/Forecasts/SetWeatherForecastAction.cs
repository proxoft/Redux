using System.Collections.Generic;
using Proxoft.Redux.BlazorApp.Shared;
using Proxoft.Redux.Core;

namespace Proxoft.Redux.BlazorApp.Client.Application.Forecasts;

public class SetWeatherForecastAction : IAction
{
    public SetWeatherForecastAction(IReadOnlyCollection<WeatherForecast> weatherForecasts)
    {
        this.WeatherForecasts = weatherForecasts;
    }

    public IReadOnlyCollection<WeatherForecast> WeatherForecasts { get; }
}
