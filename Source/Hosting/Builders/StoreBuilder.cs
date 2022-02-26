using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proxoft.Redux.Hosting.Builders
{
    public partial class StoreBuilder<TState>
    {
        private readonly ServiceLifetime _serviceLifetime;

        internal StoreBuilder(ServiceLifetime serviceLifetime)
        {
            _serviceLifetime = serviceLifetime;

            this.InitialDispatcher();
            this.InitialStateStream();
            this.InitialExceptionHandler();
        }

        internal void RegisterServices(IServiceCollection services)
        {
            this.RegisterReducer(services);
            this.RegisterDispatcher(services);
            this.RegisterStateStream(services);
            this.RegisterEffects(services);
            this.RegisterExceptionHandler(services);

            services.Add(this.ToServiceDescriptor<Store<TState>>());
        }

        private ServiceDescriptor ToServiceDescriptor<TService>()
            => new ServiceDescriptor(typeof(TService), typeof(TService), _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService, TImplementation>() where TImplementation : TService
            => new ServiceDescriptor(typeof(TService), typeof(TImplementation), _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService, TImplementation>(Func<IServiceProvider, TImplementation> resolve) where TImplementation : class, TService
            => new ServiceDescriptor(typeof(TService), resolve, _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService, TImplementation>(TImplementation instance) where TImplementation : TService
            => new ServiceDescriptor(typeof(TService), sp => instance ?? throw new Exception("cannot be null"), _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService>(Type implementation)
            => new ServiceDescriptor(typeof(TService), implementation, _serviceLifetime);
    }
}
