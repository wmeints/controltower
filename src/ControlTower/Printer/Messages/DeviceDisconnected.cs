namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     This message signals that the printer was disconnected.
    /// </summary>
    public class DeviceDisconnected
    {
        /// <summary>
        ///     Gets the static instance for the message
        /// </summary>
        public static DeviceDisconnected Instance { get; } = new DeviceDisconnected();
    }
}