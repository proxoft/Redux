using System;

namespace Proxoft.Redux.Core.Dispatchers;

public class ActionJournaler(Action<IAction> jornal) : IActionJournaler
{
    private readonly Action<IAction> _jornal = jornal;

    public void Journal(IAction action, Type? sender)
    {
        _jornal(action);
    }
}
