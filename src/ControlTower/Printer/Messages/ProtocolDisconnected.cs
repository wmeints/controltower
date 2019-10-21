namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     This message is sent by the protocol layer after it disconnected.
    /// </summary>
    public class ProtocolDisconnected
    {
        /// <summary>
        ///     Gets the static instance for the message
        /// </summary>
        public static ProtocolDisconnected Instance { get; } = new ProtocolDisconnected();
    }
}