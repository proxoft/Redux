using Proxoft.Redux.Core;

namespace Proxoft.Redux.BlazorApp.Client.Application.Counters;

public class IncreaseCounterAction(int byValue) : IAction
{
    public int ByValue { get; } = byValue;
}
