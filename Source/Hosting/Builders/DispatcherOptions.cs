using Proxoft.Redux.Core;
using System;
using System.Reactive.Concurrency;

namespace Proxoft.Redux.Hosting.Builders
{
    public class DispatcherOptions
    {
        public Action<IAction>? Journaller { get; set; }
        public IScheduler? Scheduler { get; set; }
    }
}
