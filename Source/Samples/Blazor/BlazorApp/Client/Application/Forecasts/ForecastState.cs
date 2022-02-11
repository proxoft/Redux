using System;
using System.Collections.Generic;
using BlazorApp.Shared;

namespace BlazorApp.Client.Application.Forecasts
{
    public record ForecastState
    {
        public string Status { get; init; } = "";
        public IReadOnlyCollection<WeatherForecast> WeatherForecasts { get; init; } = Array.Empty<WeatherForecast>();
    }
}
