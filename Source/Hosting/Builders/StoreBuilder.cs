using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Dispatcher;
using Proxoft.Redux.Core.ExceptionHandling;

namespace Proxoft.Redux.Hosting.Builders
{
    internal class StoreBuilder<TState> :
        IActionDispatcherBuilder<TState>,
        IReducerBuilder<TState>,
        IStateStreamBuilder<TState>,
        IStoreBuilder<TState>
    {
        private readonly IServiceCollection _services;
        private readonly ServiceLifetime _serviceLifetime;

        private ServiceDescriptor[] _actionDispatcherDescriptors = Array.Empty<ServiceDescriptor>();
        private ServiceDescriptor? _reducerDescriptor;
        private ServiceDescriptor[] _stateStreamDescriptors = Array.Empty<ServiceDescriptor>();

        private readonly List<Type> _effectTypes = new List<Type>();

        private ServiceDescriptor? _exceptionHandler;

        public StoreBuilder(IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            _services = services;
            _serviceLifetime = serviceLifetime;

            this.UseExceptionHandler(exception => throw exception);
        }

        public IReducerBuilder<TState> UseDefaultDispatcher()
            => this.UseDefaultDispatcher(_ => { });

        public IReducerBuilder<TState> UseDefaultDispatcher(Action<DispatcherOptions> configure)
        {
            DispatcherOptions options = new DispatcherOptions();
            configure(options);

            Action<IAction> emptyJournaler = _ => { };
            return this.UseDispatcher(new DefaultActionDispatcher(options.Journaller ?? emptyJournaler, options.Scheduler ?? Scheduler.CurrentThread));
        }

        public IReducerBuilder<TState> UseDispatcher<TActionDispatcher>() where TActionDispatcher : IActionDispatcher
        {
            _actionDispatcherDescriptors = new[]
            {
                this.ToServiceDescriptor<IActionDispatcher, TActionDispatcher>()
            };

            return this;
        }

        public IReducerBuilder<TState> UseDispatcher<TActionDispatcher>(TActionDispatcher actionDispatcher) where TActionDispatcher : IActionDispatcher
        {
            _actionDispatcherDescriptors = new[]
            {
                this.ToServiceDescriptor<IActionDispatcher>(actionDispatcher),
                this.ToServiceDescriptor<IActionDispatcher, TActionDispatcher>(actionDispatcher)
            };

            return this;
        }

        public IStateStreamBuilder<TState> UseReducerFunc(Func<TState, IAction, TState> func)
            => this.UseReducer(new FuncReducer<TState>(func));

        public IStateStreamBuilder<TState> UseReducer(IReducer<TState> reducer)
        {
            _reducerDescriptor = new ServiceDescriptor(typeof(IReducer<TState>), sp => reducer, _serviceLifetime);
            return this;
        }

        public IStateStreamBuilder<TState> UseReducer<TReducer>()
            where TReducer: IReducer<TState>
        {
            _reducerDescriptor = this.ToServiceDescriptor<IReducer<TState>, TReducer>();
            return this;
        }

        public IStoreBuilder<TState> UseDefaultStateStream()
            => this.UseDefaultStateStream(Scheduler.CurrentThread);

        public IStoreBuilder<TState> UseDefaultStateStream(IScheduler scheduler)
            => this.UseStateStream(new DefaultStateStream<TState>(scheduler));

        public IStoreBuilder<TState> UseStateStream<TStateStream>(TStateStream stateStreamSubject) where TStateStream : IStateStreamSubject<TState>
        {
            _stateStreamDescriptors = new[] {
                this.ToServiceDescriptor<IStateStream<TState>>(stateStreamSubject),
                this.ToServiceDescriptor<IStateStream<TState>, TStateStream>(stateStreamSubject),
                this.ToServiceDescriptor<IStateStreamSubject<TState>>(stateStreamSubject),
                this.ToServiceDescriptor<IStateStreamSubject<TState>, TStateStream>(stateStreamSubject)
            };

            return this;
        }

        public IStoreBuilder<TState> AddEffect<TEffectType>() where TEffectType: IEffect<TState>
        {
            _effectTypes.Add(typeof(TEffectType));
            return this;
        }

        public IStoreBuilder<TState> AddEffects(params Type[] effectTypes)
        {
            _effectTypes.AddRange(effectTypes);
            return this;
        }

        public IStoreBuilder<TState> AddEffects(params Assembly[] fromAssemblies)
        {
            var effectTypes = fromAssemblies
                .SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Implements<IEffect<TState>>()))
                .ToArray();

            return this.AddEffects(effectTypes);
        }

        public IStoreBuilder<TState> UseRethrowExceptionHandler()
        {
            return this.UseExceptionHandler(exception => throw exception);
        }

        public IStoreBuilder<TState> UseExceptionHandler<TExceptionHandler>()
            where TExceptionHandler : IExceptionHandler
        {
            _exceptionHandler = this.ToServiceDescriptor<IExceptionHandler, TExceptionHandler>();
            return this;
        }

        public IStoreBuilder<TState> UseExceptionHandler(Action<Exception> exceptionHandler)
        {
            _exceptionHandler = new ServiceDescriptor(
                typeof(IExceptionHandler),
                _ => {
                    return new ActionExceptionHandler(exceptionHandler);
                },
                _serviceLifetime);

            return this;
        }

        public void Register()
        {
            foreach (var sd in _stateStreamDescriptors.Concat(_actionDispatcherDescriptors))
            {
                _services.Add(sd);
            }

            if(_reducerDescriptor != null)
            {
                _services.Add(_reducerDescriptor);
            }
            

            foreach (var et in _effectTypes)
            {
                _services.Add(this.ToServiceDescriptor<IEffect<TState>>(et));
            }

            _services.Add(this.ToServiceDescriptor<Store<TState>>());

            if(_exceptionHandler != null)
            {
                _services.Add(_exceptionHandler);
            }
        }

        private ServiceDescriptor ToServiceDescriptor<TService>()
            => new ServiceDescriptor(typeof(TService), typeof(TService), _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService, TImplementation>() where TImplementation : TService
            => new ServiceDescriptor(typeof(TService), typeof(TImplementation), _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService, TImplementation>(TImplementation instance) where TImplementation : TService
            => new ServiceDescriptor(typeof(TService), sp => instance ?? throw new Exception("cannot be null"), _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService>(Type implementation)
            => new ServiceDescriptor(typeof(TService), implementation, _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService>(object instance)
            => new ServiceDescriptor(typeof(TService), sp => instance, _serviceLifetime);
    }
}
