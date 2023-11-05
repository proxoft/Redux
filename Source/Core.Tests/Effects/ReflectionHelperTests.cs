using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Effects;
using Xunit;

namespace Proxoft.Redux.Core.Tests.Effects;

public class ReflectionHelperTests
{
    [Fact]
    public void GetObservableProperties_AutoSubscribe()
    {
        Assert.Single(new TestClass().GetObservableProperties<IAction>(false));
        Assert.Empty(new TestClass().GetObservableProperties<Unit>(false));
        Assert.Equal(2, new TestClass().GetObservableProperties<object>(false).Count());
    }

    [Fact]
    public void GetObservableMethods_AutoSubscribe()
    {
        Assert.Single(new TestClass().GetObservableMethods<IAction>(false));
        Assert.Empty(new TestClass().GetObservableMethods<Unit>(false));
        Assert.Equal(2, new TestClass().GetObservableMethods<object>(false).Count());
    }

    [Fact]
    public void GetObservableProperties_ManualSubscribe()
    {
        Assert.Single(new TestClass().GetObservableProperties<IAction>(true));
        Assert.Empty(new TestClass().GetObservableProperties<Unit>(true));
        Assert.Single(new TestClass().GetObservableProperties<object>(true));
    }

    [Fact]
    public void GetObservableMethods_ManualSubscribe()
    {
        Assert.Single(new TestClass().GetObservableMethods<IAction>(true));
        Assert.Empty(new TestClass().GetObservableMethods<Unit>(true));
        Assert.Single(new TestClass().GetObservableMethods<object>(true));
    }

    [Fact]
    public void GetObservableProperty_OfArray()
    {
        Assert.Single(new TestClass2().GetObservableProperties<IAction[]>(false));
        Assert.Empty(new TestClass2().GetObservableProperties<IAction>(false));
    }

    private class TestClass2
    {
        private IObservable<IAction[]> Property1 => new Subject<IAction[]>();
    }

    private class TestClass
    {
        [Subscribe]
        private IObservable<IAction> Property1 => new Subject<IAction>();

        private IObservable<string> Property2 => new Subject<string>();

        [IgnoreSubscribe]
        private IObservable<IAction> Property3 => new Subject<IAction>();

        [Subscribe]
        private IObservable<IAction> Method1() => new Subject<IAction>();

        private IObservable<string> Method2() => new Subject<string>();

        [IgnoreSubscribe]
        private IObservable<IAction> Method3() => new Subject<IAction>();

        private IObservable<TAction> GenericMethod<TAction>() where TAction : IAction
        {
            return new Subject<TAction>();
        }
    }
}
