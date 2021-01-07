using ConsoleApp.Application.Actions;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Actions;

namespace ConsoleApp.Application
{
    public static class ApplicationReducer
    {
        public static ApplicationState Reduce(ApplicationState state, IAction action)
        {
            return action switch
            {
                InitializeAction _ => state with { Message = "Initialize Action"},
                InitializeEffectsAction _ => state with { Message = "Initialize Effects Action executed"},
                SetMessageAction messageAction => state with { Message = messageAction.Message },
                _ => state
            };
        }
    }
}
