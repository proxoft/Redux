using Proxoft.Redux.Core;

namespace ConsoleApp.Application.Actions
{
    public class FireExceptionAction : IAction
    {
        public FireExceptionAction(bool inReducer, bool inEffect)
        {
            this.InReducer = inReducer;
            this.InEffect = inEffect;
        }

        public bool InReducer { get; }
        public bool InEffect { get; }
    }
}
