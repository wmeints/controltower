namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is used by the transport layer to activate the receiver mechanism of the layer
    /// </summary>
    public class ReadFromPrinter
    {
        /// <summary>
        /// Gets the static instance for the message
        /// </summary>
        public static ReadFromPrinter Instance { get; } = new ReadFromPrinter();
    }
}