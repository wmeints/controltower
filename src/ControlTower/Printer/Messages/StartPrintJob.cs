using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is used to start a new job on the 3D printer
    /// </summary>
    public class StartPrintJob
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StartPrintJob"/>
        /// </summary>
        /// <param name="commands">Commands that are part of the job</param>
        public StartPrintJob(IEnumerable<PrinterCommand> commands)
        {
            Commands = commands;
        }

        /// <summary>
        /// Gets the parsed commands for the job
        /// </summary>
        public IEnumerable<PrinterCommand> Commands { get; }
    }
}
