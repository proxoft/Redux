using System;
using System.Reactive.Concurrency;
using System.Reflection;
using Proxoft.Redux.Core;

namespace Proxoft.Redux.Hosting.Builders
{
    public interface IActionDispatcherBuilder<TState>
    {
        IReducerBuilder<TState> UseDefaultDispatcher();
        IReducerBuilder<TState> UseDefaultDispatcher(IScheduler scheduler);
        IReducerBuilder<TState> UseDispatcher(IActionDispatcher actionDispatcher);
    }

    public interface IReducerBuilder<TState>
    {
        IStateStreamBuilder<TState> UseReducerFunc(Func<TState, IAction, TState> func);
        IStateStreamBuilder<TState> UseReducer(IReducer<TState> reducer);
    }

    public interface IStateStreamBuilder<TState>
    {
        IEffectsBuilder<TState> UseDefaultStateStream();
        IEffectsBuilder<TState> UseDefaultStateStream(IScheduler scheduler);
        IEffectsBuilder<TState> UseStateStream(IStateStreamSubject<TState> stateStreamSubject);
    }

    public interface IEffectsBuilder<TState>
    {
        IEffectsBuilder<TState> AddEffect<TEffectType>() where TEffectType : IEffect<TState>;
        IEffectsBuilder<TState> AddEffects(params Type[] effectTypes);
        IEffectsBuilder<TState> AddEffects(params Assembly[] fromAssemblies);

        IStoreBuilder<TState> Prepare();
    }

    public interface IStoreBuilder<TState>
    {
        void Build();
    }
}
