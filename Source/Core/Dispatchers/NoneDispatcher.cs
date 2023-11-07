using System;
using System.Reactive.Linq;

namespace Proxoft.Redux.Core.Dispatchers;

public class NoneDispatcher : IActionDispatcher
{
    public static readonly NoneDispatcher Instance = new();

    private readonly IObservable<IAction> _never = Observable.Never<IAction>();

    private NoneDispatcher()
    {
    }

    public void Dispatch(IAction action, Type? sender)
    {
    }

    public IDisposable Subscribe(IObserver<IAction> observer)
    {
        return _never.Subscribe(observer);
    }
}
