using System;

namespace Proxoft.Redux.Core
{
    public interface IActionJournaler
    {
        void Journal(IAction action, Type? sender);
    }
}
