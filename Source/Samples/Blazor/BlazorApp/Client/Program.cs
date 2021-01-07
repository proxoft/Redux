using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using BlazorApp.Client.Application;
using BlazorApp.Client.Application.Counters;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Proxoft.Redux.Core;

namespace BlazorApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services
                .UseRedux<ApplicationState>()
                .UseDefaultDispatcher()
                .UseReducer(new ApplicationReducer())
                .UseDefaultStateStream()
                .AddEffects(Assembly.GetExecutingAssembly())
                .Prepare()
                .Build();

            var host = builder.Build();
            var store = host.Services.GetRequiredService<Store<ApplicationState>>();

            var cs = new CounterState();

            store.Initialize(() => ApplicationState.Init);

            await host.RunAsync();
        }
    }
}
