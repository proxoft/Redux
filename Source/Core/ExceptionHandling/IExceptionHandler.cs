using System;

namespace Proxoft.Redux.Core.ExceptionHandling
{
    public interface IExceptionHandler
    {
        void OnException(Exception exception);
    }
}
