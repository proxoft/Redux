using System.Collections.Generic;
using BlazorApp.Shared;

namespace BlazorApp.Client.Application.Forecasts
{
    public record ForecastState
    {
        public string Status { get; set; }
        public IReadOnlyCollection<WeatherForecast> WeatherForecasts { get; set; }
    }
}
