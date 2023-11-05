using System;
using Proxoft.Redux.Core;

namespace Proxoft.Redux.BlazorApp.Client;

public class ActionJournaler : IActionJournaler
{
    public void Journal(IAction action, Type? sender = null)
    {
        System.Console.WriteLine($"dispatched: {action.GetType().Name}");
    }
}
