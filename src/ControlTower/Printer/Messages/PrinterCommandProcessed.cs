namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is sent by the protocol layer when a command was processed.
    /// </summary>
    public class PrinterCommandProcessed
    {
        /// <summary>
        /// Gets the static instance for the message
        /// </summary>
        public static PrinterCommandProcessed Instance { get; } = new PrinterCommandProcessed();
    }
}
