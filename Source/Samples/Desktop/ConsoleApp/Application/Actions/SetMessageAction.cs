using Proxoft.Redux.Core;

namespace ConsoleApp.Application.Actions
{
    public class SetMessageAction : IAction
    {
        public SetMessageAction(string message, string source = "")
        {
            this.Message = message;
            this.Source = source;
        }

        public string Message { get; }

        public string Source { get; }
    }
}
