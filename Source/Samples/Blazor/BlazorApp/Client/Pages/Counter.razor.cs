using System;
using System.Reactive.Linq;
using BlazorApp.Client.Application.Counters;
using Proxoft.Redux.Core.Tools;

namespace BlazorApp.Client.Pages
{
    public sealed partial class Counter : IDisposable
    {
        private SubscriptionsManager _sm = new ();
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

            _sm.AddSubscriptions(
                this.AppStream
                    .Select(a => a.Counter)
                    .DistinctUntilChanged()
                    .Subscribe(c => _state = c)
            );
        }

        public void Dispose()
        {
            _sm.Dispose();
            _sm = null;
        }
    }
}
