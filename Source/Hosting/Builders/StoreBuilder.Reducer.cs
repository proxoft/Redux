using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proxoft.Redux.Hosting.Builders
{
    public partial class StoreBuilder<TState>
    {
        private ServiceDescriptor? _reducer;

        public StoreBuilder<TState> UseReducer(Func<TState, IAction, TState> reducerFunction)
        {
            var r = new FuncReducer<TState>(reducerFunction);
            return this.UseReducer(r);
        }

        public StoreBuilder<TState> UseReducer<TReducer>(TReducer reducer)
            where TReducer : IReducer<TState>
        {
            _reducer = this.ToServiceDescriptor<IReducer<TState>, TReducer>(reducer);
            return this;
        }

        public StoreBuilder<TState> UseReducer<TReducer>()
            where TReducer : IReducer<TState>
        {
            _reducer = this.ToServiceDescriptor<IReducer<TState>, TReducer>();
            return this;
        }

        private IServiceCollection RegisterReducer(IServiceCollection services)
        {
            if (_reducer is null)
            {
                throw new Exception("There's no reducer service registered. Call builder.UseReducer<> or some of its overloads to configure the reducer");
            }

            services.Add(_reducer);
            return services;
        }
    }
}
