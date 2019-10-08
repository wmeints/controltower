using System;

namespace ControlTower.Printer
{
    /// <summary>
    /// Gets raised when the protocol layer encounters an error while processing requests/responses.
    /// </summary>
    public class ProtocolException: Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolException"/>.
        /// </summary>
        /// <param name="message">Message to display.</param>
        public ProtocolException(string message) : base(message)
        {
        }
    }
}