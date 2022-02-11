using System;
using System.Reactive.Linq;
using BlazorApp.Client.Application;
using BlazorApp.Client.Application.Forecasts;
using BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Tools;

namespace BlazorApp.Client.Pages
{
    public sealed partial class FetchData : IDisposable
    {
        private SubscriptionsManager _sm = new ();
        private ForecastState _state = new ForecastState() with {
            Status = "Init..",
            WeatherForecasts = Array.Empty<WeatherForecast>()
        };

        public void Dispose()
        {
            _sm.Dispose();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _sm.AddSubscriptions(
                this.AppStream
                    .Select(a => a.Forecast)
                    .DistinctUntilChanged()
                    .Subscribe(f => {
                        _state = f;
                        this.StateHasChanged();
                    })
            );

            this.Dispatcher.Dispatch(new FetchWeatherForcastDataAction());
        }
    }
}
