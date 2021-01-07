using System;

namespace Proxoft.Redux.Hosting.Builders
{
    internal static class ReflectionHelpers
    {
        public static bool Implements<I>(this Type type)
        {
            var interfaceType = typeof(I);
            if (!interfaceType.IsInterface)
            {
                throw new Exception("Expected interface type");
            }

            var r = interfaceType.IsAssignableFrom(type);
            return r;
        }
    }
}
