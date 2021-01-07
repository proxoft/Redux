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
        private IActionDispatcher _actionDispatcher;
        private IReducer<TState> _reducer;
        private IStateStreamSubject<TState> _stateStream;
        private readonly List<Type> _effectTypes = new();

        public StoreBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public IReducerBuilder<TState> UseDefaultDispatcher()
            => this.UseDefaultDispatcher(Scheduler.CurrentThread);

        public IReducerBuilder<TState> UseDefaultDispatcher(IScheduler scheduler = null)
            => this.UseDispatcher(new DefaultActionDispatcher(scheduler ?? Scheduler.CurrentThread));

        public IReducerBuilder<TState> UseDispatcher(IActionDispatcher actionDispatcher)
        {
            _actionDispatcher = actionDispatcher;
            return this;
        }

        public IStateStreamBuilder<TState> UseReducerFunc(Func<TState, IAction, TState> func)
        {
            _reducer = new FuncReducer<TState>(func);
            return this;
        }

        public IStateStreamBuilder<TState> UseReducer(IReducer<TState> reducer)
        {
            _reducer = reducer;
            return this;
        }

        public IEffectsBuilder<TState> UseDefaultStateStream()
            => this.UseDefaultStateStream(Scheduler.CurrentThread);

        public IEffectsBuilder<TState> UseDefaultStateStream(IScheduler scheduler)
        {
            _stateStream = new DefaultStateStream<TState>(scheduler);
            return this;
        }

        public IEffectsBuilder<TState> UseStateStream(IStateStreamSubject<TState> stateStreamSubject)
        {
            _stateStream = stateStreamSubject;
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
            _services
                .AddSingleton(_actionDispatcher)
                .AddSingleton(_reducer)
                .AddSingleton(_stateStream)
                .AddSingleton<IStateStream<TState>>(_stateStream);

            foreach(var e in _effectTypes)
            {
                _services.AddSingleton(typeof(IEffect<TState>), e);
            }

            _services.AddSingleton<Store<TState>>();
        }
    }
}
