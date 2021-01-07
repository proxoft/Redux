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
        protected override IEnumerable<IDisposable> OnConnect()
        {
            yield return this.ActionStream
                .Where(action => action is not SetMessageAction ma || ma.Source != nameof(ApplicationEffect))
                .Select(action => {
                    return action switch
                    {
                        InitializeAction _ => new SetMessageAction("From ApplicationEffect after InitializeEffect", nameof(ApplicationEffect)),
                        InitializeEffectsAction _ => new SetMessageAction("From ApplicationEffect after InitializeEffect", nameof(ApplicationEffect)),
                        SetMessageAction _ => new SetMessageAction("From ApplicationEffect after SetMessage", nameof(ApplicationEffect)),
                        _ => DefaultActions.None
                    };
                })
                .Subscribe(a => this.Dispatch(a));
        }
    }
}
