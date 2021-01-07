using Proxoft.Redux.Core;

namespace BlazorApp.Client.Application.Counters
{
    public class IncreaseCounterAction : IAction
    {
        public IncreaseCounterAction(int byValue)
        {
            this.ByValue = byValue;
        }

        public int ByValue { get; }
    }
}
