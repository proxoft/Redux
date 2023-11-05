using Proxoft.Redux.BlazorApp.Client.Application;
using Microsoft.AspNetCore.Components;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Dispatchers;
using Proxoft.Redux.Core.Tools;
using System;
using System.Reactive.Linq;

namespace Proxoft.Redux.BlazorApp.Client.Pages;

public class ClientBasePage : ComponentBase, IDisposable
{
    private SubscriptionsManager _sm = new();

    [Inject]
    public IObservable<ApplicationState> AppStream { get; set; } = Observable.Never<ApplicationState>();

    [Inject]
    public IActionDispatcher Dispatcher { get; set; } = NoneDispatcher.Instance;

    public void Dispose()
    {
        _sm.Dispose();
    }

    protected void AddSubscriptions(params IDisposable[] subscriptions)
    {
        _sm.AddSubscriptions(subscriptions);
    }
}
