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
                FireExceptionAction e when e.InReducer => throw new System.Exception("Exception thrown in reducer on behalf of action"),
                GuardedAction => state with
                {
                    GuardedActionsCount = state.GuardedActionsCount + 1
                },
                ResetGuardedAction => state with
                {
                    GuardedActionsCount = 0
                },
                _ => state
            };
        }
    }
}
