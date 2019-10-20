using System.IO.Ports;
using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    /// Implements a transport layer for the printer over serial port.
    /// <para>
    /// Continuously reads data from the serial port and delivers the responses
    /// to the connected protocol layer. 
    /// </para>
    /// <para>
    /// Write commands are processed as they come in from the protocol layer.
    /// This is an important detail of this actor, flow control is maintained in the protocol
    /// layer.
    /// </para>
    /// </summary>
    public class SerialPrinterTransport : ReceiveActor
    {
        private readonly IActorRef _protocol;
        private readonly SerialPort _port;

        /// <summary>
        /// Initializes a new instance of <see cref="SerialPrinterTransport"/>
        /// </summary>
        /// <param name="portName">Port name to use</param>
        /// <param name="baudRate">Baud rate to use for communication</param>
        /// <param name="protocol">The connected protocol layer</param>
        public SerialPrinterTransport(string portName, int baudRate, IActorRef protocol)
        {
            _protocol = protocol;
            _port = new SerialPort(portName, baudRate);

            Disconnected();
        }

        /// <summary>
        /// Creates actor properties for the <see cref="SerialPrinterTransport"/> actor.
        /// </summary>
        /// <param name="portName">Port name to connect to</param>
        /// <param name="baudRate">Baud rate to use for communicating with the printer</param>
        /// <param name="protocol">Protocol to use for handling printer communication</param>
        /// <returns>Returns the actor props</returns>
        public static Akka.Actor.Props Props(string portName, int baudRate, IActorRef protocol)
        {
            return new Props(
                typeof(SerialPrinterTransport), 
                new object[] { portName, baudRate, protocol });
        }

        /// <summary>
        /// Configures the disconnected state for the actor
        /// </summary>
        private void Disconnected()
        {
            Receive<ConnectTransport>(_ =>
            {
                _protocol.Tell(TransportConnected.Instance);

                _port.Open();

                Become(Connected);
            });
        }

        /// <summary>
        /// Configures the connected state for the actor
        /// </summary>
        private void Connected()
        {
            Receive<PrinterCommand>(WriteData);
            Receive<ReadFromPrinter>(ReadData);

            Receive<DisconnectTransport>(_ =>
            {
                _protocol.Tell(TransportDisconnected.Instance);

                _port.Close();

                Become(Disconnected);
            });
        }

        /// <summary>
        /// Handles incoming printer commands
        /// </summary>
        /// <param name="obj"></param>
        private void WriteData(PrinterCommand obj)
        {
            _port.WriteLine(obj.Serialize());
        }

        /// <summary>
        /// Reads from the serial ports and sends the result to the protocol layer.
        /// This method schedules itself again to get the next line of data from the printer.
        /// </summary>
        /// <param name="obj"></param>
        private void ReadData(ReadFromPrinter obj)
        {
            var line = _port.ReadLine();

            _protocol.Tell(new PrinterResponse(line));

            Self.Tell(ReadFromPrinter.Instance);
        }
    }
}