using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;

namespace ControlTower.Printer
{
    /// <summary>
    /// Handles printer status reports and translates to stuff that Blazor understands.
    /// </summary>
    public class PrinterMonitor: ReceiveActor
    {
    }
}
