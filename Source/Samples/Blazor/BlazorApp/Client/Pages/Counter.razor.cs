using System;
using System.Reactive.Linq;
using BlazorApp.Client.Application.Counters;
using Proxoft.Redux.Core.Tools;

namespace BlazorApp.Client.Pages
{
    public sealed partial class Counter
    {
        private CounterState _state = new ();

        private void OnClick()
        {
            this.Dispatcher.Dispatch(new IncreaseCounterAction(3));
        }

        private void OnReset()
        {
            this.Dispatcher.Dispatch(new ResetCounterAction());
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.AddSubscriptions(
                this.AppStream
                    .Select(a => a.Counter)
                    .DistinctUntilChanged()
                    .Subscribe(c => _state = c)
            );
        }
    }
}
