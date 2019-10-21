namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is sent by the protocol layer when it successfully connected to the transport layer.
    /// </summary>
    public class ProtocolConnected
    {
        /// <summary>
        /// Gets the static instance for the message
        /// </summary>
        public static ProtocolConnected Instance { get; } = new ProtocolConnected();
    }
}
