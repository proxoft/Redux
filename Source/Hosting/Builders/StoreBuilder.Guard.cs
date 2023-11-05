using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Guards;

namespace Proxoft.Redux.Hosting.Builders;

public partial class StoreBuilder<TState>
{
    private ServiceDescriptor _guardServiceDescriptor = null!;

    public StoreBuilder<TState> AddGuard<TGuard>()
        where TGuard : IGuard<TState>
    {
        _guardServiceDescriptor = this.ToServiceDescriptor<IGuard<TState>,TGuard >();
        return this;
    }

    public StoreBuilder<TState> AddGuard(IGuard<TState> guard)
    {
        _guardServiceDescriptor = this.ToServiceDescriptor(guard);
        return this;
    }

    public StoreBuilder<TState> AddGuard(Func<IAction, TState, IAction> guardFunc)
    {
        return this.AddGuard(new FuncGuard<TState>(guardFunc));
    }

    private void InitialGuard()
    {
        this.AddGuard(new FuncGuard<TState>((action, state) => action));
    }

    private void RegisterGuard(IServiceCollection services)
    {
        services.Add(_guardServiceDescriptor);
    }
}
