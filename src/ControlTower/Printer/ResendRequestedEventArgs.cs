using System;

namespace ControlTower.Printer
{
    /// <summary>
    /// Event arguments for the <see cref="Protocol.ResendRequested"/> event.
    /// </summary>
    public class ResendRequestedEventArgs: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ResendRequestedEventArgs"/>
        /// </summary>
        /// <param name="lineNumber"></param>
        public ResendRequestedEventArgs(int lineNumber)
        {
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Gets the line number from which to start resending the commands
        /// </summary>
        public int LineNumber { get; }
    }
}