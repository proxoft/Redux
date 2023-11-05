using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Proxoft.Redux.Core.Effects;

internal static class ReflectionHelper
{
    public static IEnumerable<Subscription<T>> GetObservableProperties<T>(this object self, bool optIn)
    {
        return self.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => x.PropertyType.IsGenericType)
            .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(IObservable<>))
            .Where(x => x.PropertyType.GetGenericArguments().SingleOrDefault(t => typeof(T).IsAssignableFrom(t)) != null)
            .Where(x => optIn
                ? Attribute.IsDefined(x, typeof(SubscribeAttribute))
                : !Attribute.IsDefined(x, typeof(IgnoreSubscribeAttribute)))
            .Select(x => new Subscription<T>(x, (IObservable<T>)x.GetValue(self)!));
    }

    public static IEnumerable<Subscription<T>> GetObservableMethods<T>(this object self, bool optIn)
    {
        return self.GetType()
            .GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => x.MemberType == MemberTypes.Method)
            .OfType<MethodInfo>()
            .Where(x => !x.IsSpecialName)
            .Where(x => !x.IsGenericMethod)
            .Where(x => x.ReturnType.IsGenericType)
            .Where(x => x.ReturnType.GetGenericTypeDefinition() == typeof(IObservable<>))
            .Where(x => x.ReturnType.GetGenericArguments().SingleOrDefault(t => typeof(T).IsAssignableFrom(t)) != null)
            .Where(x => !x.GetParameters().Any())
            .Where(x => optIn
                ? Attribute.IsDefined(x, typeof(SubscribeAttribute))
                : !Attribute.IsDefined(x, typeof(IgnoreSubscribeAttribute)))
            .Select(x => new Subscription<T>(x, (IObservable<T>)x.Invoke(self, Array.Empty<object?>())!));
    }
}