using System.IO.Ports;
using Microsoft.Extensions.Logging;

namespace ControlTower.Printer
{
    /// <summary>
    /// Implementation of the printer transport for connecting USB printers.
    /// </summary>
    public class SerialPrinterTransport : IPrinterTransport
    {
        private readonly ILogger<SerialPrinterTransport> _logger;
        private readonly SerialPort _port;
        private readonly string _portName;
        private readonly int _baudRate;

        /// <summary>
        /// Initializes a new instance of <see cref="SerialPrinterTransport"/>
        /// </summary>
        /// <param name="portName">Port name to bind</param>
        /// <param name="baudRate">Baud rate to use for communication</param>
        public SerialPrinterTransport(string portName, int baudRate, ILogger<SerialPrinterTransport> logger)
        {
            _portName = portName;
            _baudRate = baudRate;
            _logger = logger;
            
            _port = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 250,
                WriteTimeout = 250
            };
        }

        /// <summary>
        /// Opens the connection to the printer
        /// </summary>
        public void Connect()
        {
            if(!_port.IsOpen)
            {
                _port.Open();
                
                _logger.LogInformation(
                    "Opened serial port to {PortName} with baud rate {BaudRate}.",
                    _portName, _baudRate);
            }
            else
            {
                _logger.LogWarning("Port already open.");
            }
        }

        /// <summary>
        /// Disconnects from the printer
        /// </summary>
        public void Disconnect()
        {
            _port.Close();
            _logger.LogInformation("Closed serial port {PortName}", _portName);
        }

        public string ReadLine()
        {
            return _port.ReadLine();
        }

        /// <summary>
        /// Writes a single line of data to the transport
        /// </summary>
        /// <param name="text">Text to send</param>
        public void WriteLine(string text)
        {
            _port.WriteLine(text);
        }
        
        /// <summary>
        /// Gets whether the printer protocol needs to generate checksums for the transport
        /// </summary>
        public bool RequiresChecksum => true;
    }
}