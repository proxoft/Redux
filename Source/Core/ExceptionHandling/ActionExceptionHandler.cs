using System;

namespace Proxoft.Redux.Core.ExceptionHandling;

public class ActionExceptionHandler(Action<Exception> handlerAction) : IExceptionHandler
{
    private readonly Action<Exception> _handlerAction = handlerAction;

    public void OnException(Exception exception)
    {
        _handlerAction(exception);
    }
}
