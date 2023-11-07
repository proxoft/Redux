using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Dispatchers;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;

namespace Proxoft.Redux.Hosting.Builders;

public partial class StoreBuilder<TState>
{
    private ServiceDescriptor? _actionDispatcherDescriptor;
    private ServiceDescriptor? _journallerDescriptor;
    private ServiceDescriptor? _schedulerDescriptor;

    public StoreBuilder<TState> UseDefaultDispatcher()
    {
        return this.UseDispatcher<DefaultActionDispatcher>();
    }

    public StoreBuilder<TState> UseDispatcher<TDispatcher>()
        where TDispatcher : IActionDispatcher
    {
        _actionDispatcherDescriptor = this.ToServiceDescriptor<IActionDispatcher, TDispatcher>();
        return this;
    }

    public StoreBuilder<TState> UseDispatcher<TDispatcher>(TDispatcher dispatcher)
        where TDispatcher : IActionDispatcher
    {
        _actionDispatcherDescriptor = this.ToServiceDescriptor<IActionDispatcher, TDispatcher>(dispatcher);
        return this;
    }

    public StoreBuilder<TState> UseScheduler(IScheduler scheduler)
    {
        _schedulerDescriptor = new ServiceDescriptor(typeof(IScheduler), scheduler);
        return this;
    }

    public StoreBuilder<TState> RemoveCurrentThreadScheduler()
    {
        _schedulerDescriptor = null;
        return this;
    }

    public StoreBuilder<TState> UseNoJournaler()
    {
        var noJournaler = new ActionJournaler(_ => { });
        return this.UseJournaler(noJournaler);
    }

    public StoreBuilder<TState> UseJournaler<TActionJournaler>()
        where TActionJournaler : IActionJournaler
    {
        _journallerDescriptor = this.ToServiceDescriptor<IActionJournaler, TActionJournaler>();
        return this;
    }

    public StoreBuilder<TState> UseJournaler<TActionJournaler>(TActionJournaler journaler)
        where TActionJournaler : IActionJournaler
    {
        _journallerDescriptor = this.ToServiceDescriptor<IActionJournaler,  TActionJournaler>(journaler);
        return this;
    }

    public StoreBuilder<TState> UseJournaler<TActionJournaler>(Action<IAction> journalerAction)
        where TActionJournaler : IActionJournaler
    {
        var j = new ActionJournaler(journalerAction);
        return this.UseJournaler(j);
    }

    private void InitialDispatcher()
    {
        this.UseDefaultDispatcher();
        this.UseNoJournaler();
        _schedulerDescriptor = this.ToServiceDescriptor<IScheduler, CurrentThreadScheduler>(Scheduler.CurrentThread);
    }

    private void RegisterDispatcher(IServiceCollection services)
    {
        if(_schedulerDescriptor != null)
        {
            services.Add(_schedulerDescriptor);
        }

        services.Add(_actionDispatcherDescriptor!);
        services.Add(_journallerDescriptor!);
    }
}
