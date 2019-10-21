namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     This message is used to signal that the printer was connected.
    /// </summary>
    public class DeviceConnected
    {
        /// <summary>
        ///     Gets the static instance for the message
        /// </summary>
        public static DeviceConnected Instance { get; } = new DeviceConnected();
    }
}