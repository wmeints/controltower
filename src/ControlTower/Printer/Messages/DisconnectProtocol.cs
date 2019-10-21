namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     Message used to disconnect the protocol layer
    /// </summary>
    public class DisconnectProtocol
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="DisconnectProtocol" />
        /// </summary>
        private DisconnectProtocol()
        {
        }

        /// <summary>
        ///     Gets the static instance of the message
        /// </summary>
        public static DisconnectProtocol Instance { get; } = new DisconnectProtocol();
    }
}