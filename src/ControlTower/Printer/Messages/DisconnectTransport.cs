namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is send to the transport layer to connect it.
    /// </summary>
    public class DisconnectTransport
    {
        /// <summary>
        /// Gets the static instance for the message
        /// </summary>
        public static DisconnectTransport Instance { get; } = new DisconnectTransport();
    }
}