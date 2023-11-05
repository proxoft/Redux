using System;
using System.Collections.Generic;

namespace Proxoft.Redux.Core.Tools
{
    public sealed class SubscriptionsManager: IDisposable
    {
        private readonly List<IDisposable> _subscriptions = [];

        public void AddSubscriptions(params IDisposable[] subscriptions)
        {
            _subscriptions.AddRange(subscriptions);
        }

        public void RemoveAllSubscriptions()
        {
            foreach (var s in _subscriptions)
            {
                s.Dispose();
            }

            _subscriptions.Clear();
        }

        public void RemoveSubscriptions(params IDisposable[] subscriptions)
        {
            foreach(var s in subscriptions)
            {
                s.Dispose();
                _subscriptions.Remove(s);
            }
        }

        public void Dispose()
        {
            this.RemoveAllSubscriptions();
        }
    }
}
