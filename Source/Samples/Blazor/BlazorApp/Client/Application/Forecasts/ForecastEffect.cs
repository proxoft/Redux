﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Proxoft.Redux.BlazorApp.Shared;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Extensions;

namespace Proxoft.Redux.BlazorApp.Client.Application.Forecasts;

public class ForecastEffect(HttpClient httpClient) : BaseApplicationEffect
{
    private readonly HttpClient _httpClient = httpClient;

    private IObservable<IAction> FetchDataEffect => this.ActionStream
        .OfType<FetchWeatherForcastDataAction>()
        .SelectAsync(action => this.FetchData(action))
        .Select(data => new SetWeatherForecastAction(data));

    private async Task<WeatherForecast[]> FetchData(FetchWeatherForcastDataAction action)
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast")
                ?? [];

            return data;
        }
        catch
        {
            return [];
        }
    }
}
