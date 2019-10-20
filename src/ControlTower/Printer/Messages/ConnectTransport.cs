namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// Message used to connect the transport layer
    /// </summary>
    public class ConnectTransport
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConnectTransport"/>
        /// </summary>
        private ConnectTransport()
        {

        }

        /// <summary>
        /// Gets the static instance for the message
        /// </summary>
        public static ConnectTransport Instance { get; } = new ConnectTransport();
    }
}