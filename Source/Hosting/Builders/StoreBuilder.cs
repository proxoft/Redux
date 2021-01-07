using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Dispatcher;

namespace Proxoft.Redux.Hosting.Builders
{
    internal class StoreBuilder<TState> :
        IActionDispatcherBuilder<TState>,
        IReducerBuilder<TState>,
        IStateStreamBuilder<TState>,
        IEffectsBuilder<TState>,
        IStoreBuilder<TState>
    {
        private readonly IServiceCollection _services;
        private readonly ServiceLifetime _serviceLifetime;

        private ServiceDescriptor[] _actionDispatcherDescriptors;
        private ServiceDescriptor _reducerDescriptor;
        private ServiceDescriptor[] _stateStreamDescriptors;

        private readonly List<Type> _effectTypes = new();

        public StoreBuilder(IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            _services = services;
            _serviceLifetime = serviceLifetime;
        }

        public IReducerBuilder<TState> UseDefaultDispatcher()
            => this.UseDefaultDispatcher(Scheduler.CurrentThread);

        public IReducerBuilder<TState> UseDefaultDispatcher(IScheduler scheduler = null)
            => this.UseDispatcher(new DefaultActionDispatcher(scheduler ?? Scheduler.CurrentThread));

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

        public IEffectsBuilder<TState> UseDefaultStateStream()
            => this.UseDefaultStateStream(Scheduler.CurrentThread);

        public IEffectsBuilder<TState> UseDefaultStateStream(IScheduler scheduler)
            => this.UseStateStream(new DefaultStateStream<TState>(scheduler));

        public IEffectsBuilder<TState> UseStateStream<TStateStream>(TStateStream stateStreamSubject) where TStateStream : IStateStreamSubject<TState>
        {
            _stateStreamDescriptors = new[] {
                this.ToServiceDescriptor<IStateStream<TState>>(stateStreamSubject),
                this.ToServiceDescriptor<IStateStream<TState>, TStateStream>(stateStreamSubject),
                this.ToServiceDescriptor<IStateStreamSubject<TState>>(stateStreamSubject),
                this.ToServiceDescriptor<IStateStreamSubject<TState>, TStateStream>(stateStreamSubject)
            };

            return this;
        }

        public IEffectsBuilder<TState> AddEffect<TEffectType>() where TEffectType: IEffect<TState>
        {
            _effectTypes.Add(typeof(TEffectType));
            return this;
        }

        public IEffectsBuilder<TState> AddEffects(params Type[] effectTypes)
        {
            _effectTypes.AddRange(effectTypes);
            return this;
        }

        public IEffectsBuilder<TState> AddEffects(params Assembly[] fromAssemblies)
        {
            var effectTypes = fromAssemblies
                .SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Implements<IEffect<TState>>()))
                .ToArray();

            return this.AddEffects(effectTypes);
        }

        public IStoreBuilder<TState> Prepare()
        {
            return this;
        }

        public void Build()
        {
            foreach(var sd in _stateStreamDescriptors.Concat(_actionDispatcherDescriptors))
            {
                _services.Add(sd);
            }

            _services.Add(_reducerDescriptor);

            foreach(var et in _effectTypes)
            {
                _services.Add(this.ToServiceDescriptor<IEffect<TState>>(et));
            }

            _services.Add(this.ToServiceDescriptor<Store<TState>>());
        }

        private ServiceDescriptor ToServiceDescriptor<TService>()
            => new (typeof(TService), typeof(TService), _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService, TImplementation>() where TImplementation : TService
            => new (typeof(TService), typeof(TImplementation), _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService, TImplementation>(TImplementation instance) where TImplementation : TService
            => new(typeof(TService), sp => instance, _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService>(Type implementation)
            => new (typeof(TService), implementation, _serviceLifetime);

        private ServiceDescriptor ToServiceDescriptor<TService>(object instance)
            => new (typeof(TService), sp => instance, _serviceLifetime);
    }
}
