using Proxoft.Redux.Hosting.Builders;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ReduxBuilder
    {
        public static IActionDispatcherBuilder<TState> UseRedux<TState>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            return new StoreBuilder<TState>(services, serviceLifetime);
        }
    }
}
