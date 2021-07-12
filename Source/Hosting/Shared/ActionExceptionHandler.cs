using System;
using Proxoft.Redux.Core.ExceptionHandling;

namespace Proxoft.Redux.Hosting
{
    public class ActionExceptionHandler : IExceptionHandler
    {
        private readonly Action<Exception> _handlerAction;

        public ActionExceptionHandler(Action<Exception> handlerAction)
        {
            _handlerAction = handlerAction;
        }

        public void OnException(Exception exception)
        {
            _handlerAction(exception);
        }
    }
}
