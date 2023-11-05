using System.Collections.Generic;
using Proxoft.Redux.BlazorApp.Shared;
using Proxoft.Redux.Core;

namespace Proxoft.Redux.BlazorApp.Client.Application.Forecasts;

public class SetWeatherForecastAction(IReadOnlyCollection<WeatherForecast> weatherForecasts) : IAction
{
    public IReadOnlyCollection<WeatherForecast> WeatherForecasts { get; } = weatherForecasts;
}
