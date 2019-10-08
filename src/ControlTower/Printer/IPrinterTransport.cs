namespace ControlTower.Printer
{
    /// <summary>
    /// A printer transport is used to send/receive data to/from the 3D printer. It handles the connection
    /// over protocols like TCP/IP and USB serial ports.
    /// </summary>
    public interface IPrinterTransport
    {
        /// <summary>
        /// Opens the connection to the printer
        /// </summary>
        void Connect();
        
        /// <summary>
        /// Disconnects from the printer
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Reads data from the transport until a linefeed or carriage return character is found.
        /// </summary>
        /// <returns>Returns the retrieved data</returns>
        string ReadLine();

        /// <summary>
        /// Writes a single line of data to the transport
        /// </summary>
        /// <param name="text">Text to send</param>
        void WriteLine(string text);

        /// <summary>
        /// Gets whether the printer protocol needs to generate checksums for the transport
        /// </summary>
        bool RequiresChecksum { get; }
    }
}