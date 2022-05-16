using System;
using System.Reactive.Linq;
using ConsoleApp.Application.Actions;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Actions;

namespace ConsoleApp.Application.Effects
{
    public class NoAutoSubscribeEffect : Effect<ApplicationState>
    {
        [IgnoreSubscribe]
        private IObservable<IAction> ShouldNotDispatch => this.ActionStream
            .Do(action => Console.WriteLine("!!!!this line should be executed!!!"))
            .Select(action => action switch
            {
                InitializeEffectsAction _ => new SetMessageAction("this action should not be dispatched", nameof(NoAutoSubscribeEffect)),
                _ => DefaultActions.None
            })
            .Where(action => action != DefaultActions.None);
    }
}