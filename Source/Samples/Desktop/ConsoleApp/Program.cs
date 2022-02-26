using System;
using System.Reflection;
using ConsoleApp.Application;
using ConsoleApp.Application.Actions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proxoft.Redux.Core;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Redux");

            var host = CreateHostBuilder(args)
                .Build();

            var store = host.Services.GetRequiredService<Store<ApplicationState>>();

            store.Initialize(() => new ApplicationState() { Message = "initial state" });

            var stream = host.Services.GetRequiredService<IStateStream<ApplicationState>>();

            stream.Subscribe(appState => Console.WriteLine(appState.Message));

            var dispatcher = host.Services.GetRequiredService<IActionDispatcher>();
            dispatcher.Dispatch(new SetMessageAction("Message dispatched from Program.Main"));

            // choose where the exception should be thrown
            dispatcher.Dispatch(new FireExceptionAction(false, false));

            dispatcher.Dispatch(new SetMessageAction("Message after the exception action has been dispatched. Program flow should reach this point if any of the arguments was true", source: "program"));

            host.Dispose();

            Console.WriteLine("Goodbye Redux");

#if !DEBUG
            Console.WriteLine("");
            Console.WriteLine("Press ENTER to end program...");
            Console.ReadLine();
#endif
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddRedux<ApplicationState>(builder =>
                    {
                        builder.UseReducer(ApplicationReducer.Reduce)
                            .AddEffects(Assembly.GetExecutingAssembly())
                            .UseExceptionHandler(e =>
                            {
                                Console.WriteLine("Exception handler");
                                Console.WriteLine(e.Message);
                            });
                    });
                });

            return host;
        }
    }
}
