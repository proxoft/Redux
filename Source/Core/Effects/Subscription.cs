using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reflection;

namespace Proxoft.Redux.Core
{
    public class Subscription<T> : Subscription
    {
        public Subscription(MemberInfo memberInfo, IObservable<T> observable) : base(memberInfo)
        {
            Observable = observable.Do(_ => Debug.WriteLine($"Running from subscription {PropertyName}"));
        }

        public IObservable<T> Observable { get; }
    }

    public class Subscription
    {
        public Subscription(MemberInfo memberInfo)
        {
            PropertyName = memberInfo.Name;
            ClassFullName = memberInfo.DeclaringType?.FullName ?? string.Empty;
            ClassName = memberInfo.DeclaringType?.Name ?? string.Empty;
        }

        public string PropertyName { get; }

        public string ClassName { get; }

        public string ClassFullName { get; }
    }
}