using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BlazorApp.Shared;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Extensions;

namespace BlazorApp.Client.Application.Forecasts
{
    public class ForecastEffect : BaseApplicationEffect
    {
        private readonly HttpClient _httpClient;

        public ForecastEffect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private IDisposable FetchDataEffect => this.ActionStream
            .OfType<FetchWeatherForcastDataAction>()
            .SelectAsync(action => this.FetchData(action))
            .Subscribe(data => this.Dispatch(new SetWeatherForecastAction(data)));

        protected override IEnumerable<IDisposable> OnConnect()
        {
            yield return this.FetchDataEffect;
        }

        private async Task<WeatherForecast[]> FetchData(FetchWeatherForcastDataAction action)
        {
            try
            {
                var data = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast")
                    ?? Array.Empty<WeatherForecast>();

                return data;
            }
            catch
            {
                return Array.Empty<WeatherForecast>();
            }
        }
    }
}
