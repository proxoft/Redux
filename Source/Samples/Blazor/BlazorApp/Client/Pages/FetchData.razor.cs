using System;
using System.Reactive.Linq;
using BlazorApp.Client.Application;
using BlazorApp.Client.Application.Forecasts;
using BlazorApp.Shared;
using Proxoft.Redux.Core.Tools;

namespace BlazorApp.Client.Pages
{
    public sealed partial class FetchData
    {
        
        private ForecastState _state = new ForecastState() with {
            Status = "Init..",
            WeatherForecasts = Array.Empty<WeatherForecast>()
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.AddSubscriptions(
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
