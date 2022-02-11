using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Linq;
using BlazorApp.Shared;
using Proxoft.Redux.Core;

namespace BlazorApp.Client.Application.Forecasts
{
    public class ForecastEffect : BaseApplicationEffect
    {
        private readonly HttpClient _httpClient;

        public ForecastEffect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected override IEnumerable<IDisposable> OnConnect()
        {
            yield return this.FetchData(this.ActionStream);
        }

        private IDisposable FetchData(IObservable<IAction> actionStream)
        {
            return actionStream
                .OfType<FetchWeatherForcastDataAction>()
                .Select(_ => Observable.FromAsync(() => _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast")))
                .Merge()
                .Subscribe(data => this.Dispatch(new SetWeatherForecastAction(data ?? Array.Empty<WeatherForecast>())));
        }
    }
}
