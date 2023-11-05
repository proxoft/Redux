using Proxoft.Redux.Core;

namespace Proxoft.Redux.BlazorApp.Client.Application.Counters;

public class IncreaseCounterAction : IAction
{
    public IncreaseCounterAction(int byValue)
    {
        this.ByValue = byValue;
    }

    public int ByValue { get; }
}
