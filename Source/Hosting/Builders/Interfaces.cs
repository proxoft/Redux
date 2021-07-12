using System;
using System.Reactive.Concurrency;
using System.Reflection;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.ExceptionHandling;

namespace Proxoft.Redux.Hosting.Builders
{
    public interface IActionDispatcherBuilder<TState>
    {
        IReducerBuilder<TState> UseDefaultDispatcher();
        IReducerBuilder<TState> UseDefaultDispatcher(IScheduler scheduler);
        IReducerBuilder<TState> UseDispatcher<TActionDispatcher>() where TActionDispatcher : IActionDispatcher;
        IReducerBuilder<TState> UseDispatcher<TActionDispatcher>(TActionDispatcher actionDispatcher) where TActionDispatcher : IActionDispatcher;
    }

    public interface IReducerBuilder<TState>
    {
        IStateStreamBuilder<TState> UseReducerFunc(Func<TState, IAction, TState> func);
        IStateStreamBuilder<TState> UseReducer(IReducer<TState> reducer);
        IStateStreamBuilder<TState> UseReducer<TReducer>() where TReducer: IReducer<TState>;
    }

    public interface IStateStreamBuilder<TState>
    {
        IStoreBuilder<TState> UseDefaultStateStream();
        IStoreBuilder<TState> UseDefaultStateStream(IScheduler scheduler);
        IStoreBuilder<TState> UseStateStream<TStateStream>(TStateStream stateStreamSubject) where TStateStream: IStateStreamSubject<TState>;
    }

    //public interface IEffectsBuilder<TState>
    //{
    //    IEffectsBuilder<TState> AddEffect<TEffectType>() where TEffectType : IEffect<TState>;
    //    IEffectsBuilder<TState> AddEffects(params Type[] effectTypes);
    //    IEffectsBuilder<TState> AddEffects(params Assembly[] fromAssemblies);

    //    IStoreBuilder<TState> Prepare();
    //}

    public interface IStoreBuilder<TState>
    {
        IStoreBuilder<TState> AddEffect<TEffectType>() where TEffectType : IEffect<TState>;
        IStoreBuilder<TState> AddEffects(params Type[] effectTypes);
        IStoreBuilder<TState> AddEffects(params Assembly[] fromAssemblies);

        IStoreBuilder<TState> UseRethrowExceptionHandler();
        IStoreBuilder<TState> UseExceptionHandler<TExceptionHandler>() where TExceptionHandler : IExceptionHandler;
        IStoreBuilder<TState> UseExceptionHandler(Action<Exception> exceptionHandler);

        void Register();
    }
}
