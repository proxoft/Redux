using Proxoft.Redux.Core;
using Proxoft.Redux.Hosting.Builders;

namespace Microsoft.Extensions.DependencyInjection;

public static class ReduxBuilder
{
    public static IServiceCollection AddRedux<TState, TReducer>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TReducer: IReducer<TState>
    {
        services.AddRedux<TState>(b => {
            b.UseReducer<TReducer>();
        });

        return services;
    }

    public static IServiceCollection AddRedux<TState>(
        this IServiceCollection services,
        System.Action<StoreBuilder<TState>> builder,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        var b = new StoreBuilder<TState>(serviceLifetime);
        builder(b);
        b.RegisterServices(services);
        return services;
    }
}
