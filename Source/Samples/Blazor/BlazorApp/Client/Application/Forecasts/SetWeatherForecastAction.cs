using System.Collections.Generic;
using BlazorApp.Shared;
using Proxoft.Redux.Core;

namespace BlazorApp.Client.Application.Forecasts
{
    public class SetWeatherForecastAction : IAction
    {
        public SetWeatherForecastAction(IReadOnlyCollection<WeatherForecast> weatherForecasts)
        {
            this.WeatherForecasts = weatherForecasts;
        }

        public IReadOnlyCollection<WeatherForecast> WeatherForecasts { get; }
    }
}
