using System;

namespace Proxoft.Redux.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoSubscribeAttribute : Attribute
    {
        public AutoSubscribeAttribute()
        {
            this.Properties = true;
            this.Methods = true;
        }

        public AutoSubscribeAttribute(bool properties, bool methods)
        {
            this.Properties = properties;
            this.Methods = methods;
        }

        public bool Properties { get; }

        public bool Methods { get; }
    }
}