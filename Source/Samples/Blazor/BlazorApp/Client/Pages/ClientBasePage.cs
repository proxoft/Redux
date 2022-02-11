using BlazorApp.Client.Application;
using Microsoft.AspNetCore.Components;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Dispatchers;
using System;
using System.Reactive.Linq;

namespace BlazorApp.Client.Pages
{
    public class ClientBasePage : ComponentBase
    {
        [Inject]
        public IObservable<ApplicationState> AppStream { get; set; } = Observable.Never<ApplicationState>();

        [Inject]
        public IActionDispatcher Dispatcher { get; set; } = NoneDispatcher.Instance;
    }
}
