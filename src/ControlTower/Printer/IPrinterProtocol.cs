using System;

namespace ControlTower.Printer
{
    /// <summary>
    /// The printer protocol is used to translate commands from the application layer to the transport layer
    /// connected to the 3D printer.
    /// </summary>
    public interface IPrinterProtocol
    {
        /// <summary>
        /// Gets raised when a command was successfully processed.
        /// </summary>
        event EventHandler CommandProcessed;

        /// <summary>
        /// Connects the protocol to the transport layer and starts the read/write logic.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the transport layer and stops processing commands and replies.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends a command to the printer and waits for a reply
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="line">Line number (optional)</param>
        /// <param name="calculateChecksum">Whether a checksum should be calculated</param>
        void SendCommand(string text, int line = -1, bool calculateChecksum = false);
    }
}