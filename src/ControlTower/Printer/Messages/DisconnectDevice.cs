namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     Message used to disconnect the device
    /// </summary>
    public class DisconnectDevice
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="DisconnectDevice" />
        /// </summary>
        private DisconnectDevice()
        {
        }

        /// <summary>
        ///     Gets the static instance of the message
        /// </summary>
        public static DisconnectDevice Instance { get; } = new DisconnectDevice();
    }
}