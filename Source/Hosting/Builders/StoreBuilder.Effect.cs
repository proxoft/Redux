using Microsoft.Extensions.DependencyInjection;
using Proxoft.Redux.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Proxoft.Redux.Hosting.Builders;

public partial class StoreBuilder<TState>
{
    private readonly List<Type> _effectTypes = new List<Type>();

    public StoreBuilder<TState> AddEffect<TEffectType>() where TEffectType : IEffect<TState>
    {
        _effectTypes.Add(typeof(TEffectType));
        return this;
    }

    public StoreBuilder<TState> AddEffects(params Type[] effectTypes)
    {
        _effectTypes.AddRange(effectTypes);
        return this;
    }

    public StoreBuilder<TState> AddEffects(params Assembly[] fromAssemblies)
    {
        var effectTypes = fromAssemblies
            .SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Implements<IEffect<TState>>()))
            .ToArray();

        return this.AddEffects(effectTypes);
    }

    private void RegisterEffects(IServiceCollection services)
    {
        foreach (var et in _effectTypes)
        {
            services.Add(this.ToServiceDescriptor<IEffect<TState>>(et));
        }
    }
}
