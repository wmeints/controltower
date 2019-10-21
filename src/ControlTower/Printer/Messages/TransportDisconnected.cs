namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// Is sent by the transport layer when it's disconnected.
    /// </summary>
    public class TransportDisconnected
    {
        /// <summary>
        /// Gets the static instance for the message
        /// </summary>
        public static TransportDisconnected Instance { get; } = new TransportDisconnected();
    }
}
