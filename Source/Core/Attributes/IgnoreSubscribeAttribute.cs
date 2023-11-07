using System;

namespace Proxoft.Redux.Core;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class IgnoreSubscribeAttribute : Attribute
{
}