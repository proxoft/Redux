using System;
using Proxoft.Redux.BlazorApp.Client.Application.Counters;
using Proxoft.Redux.BlazorApp.Client.Application.Forecasts;
using Proxoft.Redux.BlazorApp.Shared;

namespace Proxoft.Redux.BlazorApp.Client.Application;

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

    public CounterState Counter { get; init; } = new();

    public ForecastState Forecast { get; init; } = new();
}
