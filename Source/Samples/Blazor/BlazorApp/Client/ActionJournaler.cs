﻿using System;
using Proxoft.Redux.Core;

namespace BlazorApp.Client
{
    public class ActionJournaler : IActionJournaler
    {
        public void Journal(IAction action, Type? sender = null)
        {
            System.Console.WriteLine($"dispatched: {action.GetType().Name}");
        }
    }
}
