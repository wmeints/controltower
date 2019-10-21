namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     Message used to connect to a 3D printer.
    /// </summary>
    public class ConnectDevice
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="ConnectDevice" />
        /// </summary>
        /// <param name="portName">Port to connect to</param>
        /// <param name="baudRate">Baud rate to use</param>
        public ConnectDevice(string portName, int baudRate)
        {
            PortName = portName;
            BaudRate = baudRate;
        }

        /// <summary>
        ///     Gets the port name
        /// </summary>
        public string PortName { get; }

        /// <summary>
        ///     Gets the baud rate
        /// </summary>
        public int BaudRate { get; }
    }
}