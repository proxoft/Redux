using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core;

namespace Proxoft.Redux.Hosting.Builders;

public partial class StoreBuilder<TState>
{
    private ServiceDescriptor[] _stateStreamDescriptors = Array.Empty<ServiceDescriptor>();

    public StoreBuilder<TState> UseDefaultStateStream()
      => this.UseDefaultStateStream(Scheduler.CurrentThread);

    public StoreBuilder<TState> UseDefaultStateStream(IScheduler scheduler)
        => this.UseStateStream(new DefaultStateStream<TState>(scheduler));

    public StoreBuilder<TState> UseStateStream<TStateStream>(TStateStream stateStreamSubject) where TStateStream : IStateStreamSubject<TState>
    {
        _stateStreamDescriptors = new[] {
            this.ToServiceDescriptor<IObservable<TState>, TStateStream>(stateStreamSubject),
            this.ToServiceDescriptor<IStateStream<TState>, TStateStream>(stateStreamSubject),
            this.ToServiceDescriptor<IStateStreamSubject<TState>, TStateStream>(stateStreamSubject)
        };

        return this;
    }

    public StoreBuilder<TState> UseStateStream<TStateStream>() where TStateStream : class, IStateStreamSubject<TState>
    {
        _stateStreamDescriptors = new[] {
            this.ToServiceDescriptor<TStateStream>(),
            this.ToServiceDescriptor<IStateStream<TState>, TStateStream>(r => r.GetRequiredService<TStateStream>()),
            this.ToServiceDescriptor<IStateStreamSubject<TState>, TStateStream>(r => r.GetRequiredService<TStateStream>())
        };

        return this;
    }

    private void InitialStateStream()
    {
        this.UseDefaultStateStream();
    }

    private void RegisterStateStream(IServiceCollection services)
    {
        foreach(var sd in _stateStreamDescriptors)
        {
            services.Add(sd);
        }
    }
}
