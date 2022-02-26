using System;
using System.Collections.Generic;
using System.Text;

namespace Proxoft.Redux.Core
{
    public interface IActionJournaler
    {
        void Journal(IAction action);
    }
}
