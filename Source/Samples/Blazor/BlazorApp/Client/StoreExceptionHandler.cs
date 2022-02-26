using Proxoft.Redux.Core.ExceptionHandling;
using System;

namespace BlazorApp.Client
{
    public class StoreExceptionHandler : IExceptionHandler
    {
        public void OnException(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}
