using Proxoft.Redux.Core;

namespace ConsoleApp.Application.Actions
{
    public class SetMessageAction(
        string message,
        string source = "") : IAction
    {
        public string Message { get; } = message;

        public string Source { get; } = source;
    }
}
