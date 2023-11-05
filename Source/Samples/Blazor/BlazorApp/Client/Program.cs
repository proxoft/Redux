using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using Proxoft.Redux.BlazorApp.Client.Application;
using Proxoft.Redux.BlazorApp.Client.Application.Counters;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Proxoft.Redux.Core;

namespace Proxoft.Redux.BlazorApp.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services
            .AddRedux<ApplicationState>(rb =>
            {
                rb.UseReducer<ApplicationReducer>()
                    .UseJournaler<ActionJournaler>()
                    .AddEffects(Assembly.GetExecutingAssembly())
                    .UseExceptionHandler<StoreExceptionHandler>()
                ;
            },
            ServiceLifetime.Scoped);

        var host = builder.Build();
        var store = host.Services.GetRequiredService<Store<ApplicationState>>();
        store.Initialize(() => ApplicationState.Init);

        await host.RunAsync();
    }
}
