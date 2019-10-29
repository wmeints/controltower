using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     This message is sent to the printer when all commands of a job have been processed.
    /// </summary>
    public class PrintJobCompleted
    {
        /// <summary>
        ///     Gets the static instance of the message
        /// </summary>
        public static PrintJobCompleted Instance { get; } = new PrintJobCompleted();
    }
}
