using ConsoleApp.Application.Actions;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Actions;
using Proxoft.Redux.Core.Guards;

namespace ConsoleApp.Application;

internal class ActionGuard : IActionGuard<ApplicationState>
{
    public IAction Validate(IAction action, ApplicationState state)
    {
        return action switch
        {
            GuardedAction when state is {  GuardedActionsCount: 1 } => DefaultActions.None,
            _ => action
        };
    }
}
