using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer
{
    /// <summary>
    /// Defines the possible state values for the print job
    /// </summary>
    public enum PrintJobState
    {
        None,
        Ready,
        Running,
        Completed,
        Cancelled
    }
}
