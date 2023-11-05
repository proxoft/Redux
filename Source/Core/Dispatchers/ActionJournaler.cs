using System;

namespace Proxoft.Redux.Core.Dispatchers;

public class ActionJournaler : IActionJournaler
{
    private readonly Action<IAction> _jornal;

    public ActionJournaler(Action<IAction> jornal)
    {
        _jornal = jornal;
    }

    public void Journal(IAction action, Type? sender)
    {
        _jornal(action);
    }
}
