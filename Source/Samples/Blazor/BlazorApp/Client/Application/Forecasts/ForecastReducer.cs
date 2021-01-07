using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BlazorApp.Shared;
using Proxoft.Redux.Core;

namespace BlazorApp.Client.Application.Forecasts
{
    public static class ForecastReducer
    {
        public static ForecastState Reduce(ForecastState state, IAction action)
        {
            return action switch
            {
                FetchWeatherForcastDataAction _ => state with { 
                    Status = "Loading...", WeatherForecasts = Array.Empty<WeatherForecast>()
                },
                SetWeatherForecastAction sfa => SetData(state, sfa.WeatherForecasts),
                _ => state
            };
        }

        private static ForecastState SetData(ForecastState state, IReadOnlyCollection<WeatherForecast> data)
        {
            var f = state with
            {
                Status = "Loaded",
                WeatherForecasts = data.ToArray()
            };

            return f;
        }
    }
}
