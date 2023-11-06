using System;

namespace Proxoft.Redux.Core.ExceptionHandling;

public class ReduxException(Subscription subscription, Exception e) : Exception(CreateMessage(subscription), e)
{
    private static string CreateMessage(Subscription subscription)
    {
        return
@$"
Exception thrown in subscription:
    Property name:   {subscription.PropertyName}
    Class name:      {subscription.ClassName}
    Class full name: {subscription.ClassFullName}";
    }
}
