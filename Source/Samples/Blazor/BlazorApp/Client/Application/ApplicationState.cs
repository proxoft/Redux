using System;
using BlazorApp.Client.Application.Counters;
using BlazorApp.Client.Application.Forecasts;
using BlazorApp.Shared;

namespace BlazorApp.Client.Application
{
    public record ApplicationState
    {
        public static readonly ApplicationState Init = new ApplicationState() with
        {
            Counter = new CounterState(),
            Forecast = new ForecastState() with
            {
                Status = "Init",
                WeatherForecasts = Array.Empty<WeatherForecast>()
            }
        };

        public CounterState Counter { get; set; }

        public ForecastState Forecast { get; set; }
    }
}
