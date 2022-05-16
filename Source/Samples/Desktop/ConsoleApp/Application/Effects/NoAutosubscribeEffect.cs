using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Application.Actions;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Actions;

namespace ConsoleApp.Application.Effects
{
    public class NoAutosubscribeEffect : Effect<ApplicationState>
    {
        [IgnoreSubscribe]
        private IObservable<IAction> ShouldNotDispatch => this.ActionStream
            .Do(action => Console.WriteLine("!!!!this line should be executed!!!"))
            .Select(action => action switch
            {
                InitializeEffectsAction _ => new SetMessageAction("this action should not be dispatched", nameof(NoAutosubscribeEffect)),
                _ => DefaultActions.None
            })
            .Where(action => action != DefaultActions.None);
    }
}