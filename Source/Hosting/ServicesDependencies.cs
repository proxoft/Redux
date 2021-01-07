using Proxoft.Redux.Hosting.Builders;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ReduxBuilder
    {
        public static IActionDispatcherBuilder<TState> UseRedux<TState>(this IServiceCollection services)
        {
            return new StoreBuilder<TState>(services);
        }
    }
}
