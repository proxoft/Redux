using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using ConsoleApp.Application.Actions;
using Proxoft.Redux.Core;
using Proxoft.Redux.Core.Actions;

namespace ConsoleApp.Application
{
    public class ApplicationEffect : Effect<ApplicationState>
    {
        private IObservable<IAction> Effect => this.ActionStream
                .Where(action => action is not SetMessageAction ma || ma.Source != nameof(ApplicationEffect))
                .Select(action =>
                {
                    return action switch
                    {
                        InitializeAction _ => new SetMessageAction("From ApplicationEffect after InitializeEffect", nameof(ApplicationEffect)),
                        InitializeEffectsAction _ => new SetMessageAction("From ApplicationEffect after InitializeEffect", nameof(ApplicationEffect)),
                        SetMessageAction _ => new SetMessageAction("From ApplicationEffect after SetMessage", nameof(ApplicationEffect)),
                        FireExceptionAction e when e.InEffect => throw new Exception("Exception thrown in effect on behalf of action"),
                        _ => DefaultActions.None
                    };
                })
                .Where(action => action != DefaultActions.None);

        protected override IEnumerable<IDisposable> OnConnect()
        {
            yield return this.SubscribeDispatch(this.Effect);
        }
    }
}
