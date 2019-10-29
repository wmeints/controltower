using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is used to report the state of the print job
    /// </summary>
    public class PrintJobStatusUpdated
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PrintJobStatusUpdated"/>
        /// </summary>
        /// <param name="state">New state of the job</param>
        public PrintJobStatusUpdated(PrintJobState state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the new state of the job
        /// </summary>
        public PrintJobState State { get; }
    }
}
