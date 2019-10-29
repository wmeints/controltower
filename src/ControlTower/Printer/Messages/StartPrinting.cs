using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is used to start sending print commands from the print job to the printer device.
    /// </summary>
    public class StartPrinting
    {
        /// <summary>
        /// Gets the static instance of the message
        /// </summary>
        public static StartPrinting Instance { get; } = new StartPrinting();
    }
}
