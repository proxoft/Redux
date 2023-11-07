using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reflection;

namespace Proxoft.Redux.Core;

public class Subscription<T> : Subscription
{
    public Subscription(MemberInfo memberInfo, IObservable<T> observable) : base(memberInfo)
    {
        this.Observable = observable;
    }

    public IObservable<T> Observable { get; }
}

public class Subscription
{
    public Subscription(MemberInfo memberInfo)
    {
        this.PropertyName = memberInfo.Name;
        this.ClassFullName = memberInfo.DeclaringType?.FullName ?? string.Empty;
        this.ClassName = memberInfo.DeclaringType?.Name ?? string.Empty;
    }

    public string PropertyName { get; }

    public string ClassName { get; }

    public string ClassFullName { get; }
}