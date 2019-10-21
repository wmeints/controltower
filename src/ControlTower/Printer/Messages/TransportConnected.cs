namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     Is sent by the transport layer when its connected.
    /// </summary>
    public class TransportConnected
    {
        /// <summary>
        ///     Gets the static instance for the message
        /// </summary>
        public static TransportConnected Instance { get; } = new TransportConnected();
    }
}