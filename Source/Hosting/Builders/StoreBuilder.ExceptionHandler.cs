using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proxoft.Redux.Hosting.Builders;

public partial class StoreBuilder<TState>
{
    private ServiceDescriptor? _exceptionHandler;

    public StoreBuilder<TState> UseRethrowExceptionHandler()
    {
        return this.UseExceptionHandler(exception => throw exception);
    }

    public StoreBuilder<TState> UseExceptionHandler<TExceptionHandler>()
        where TExceptionHandler : IExceptionHandler
    {
        _exceptionHandler = this.ToServiceDescriptor<IExceptionHandler, TExceptionHandler>();
        return this;
    }

    public StoreBuilder<TState> UseExceptionHandler(Action<Exception> exceptionHandler)
    {
        _exceptionHandler = new ServiceDescriptor(
            typeof(IExceptionHandler),
            _ => {
                return new ActionExceptionHandler(exceptionHandler);
            },
            _serviceLifetime);

        return this;
    }

    private void InitialExceptionHandler()
    {
        this.UseRethrowExceptionHandler();
    }

    private void RegisterExceptionHandler(IServiceCollection services)
    {
        services.Add(_exceptionHandler!);
    }
}
