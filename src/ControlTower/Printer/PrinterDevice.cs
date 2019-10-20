using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.IO;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    /// Implementation of the printer's application layer.
    /// </summary>
    public class PrinterDevice : ReceiveActor, IWithUnboundedStash
    {
        private readonly IActorRef _protocol;
        private readonly IActorRef _monitor;

        /// <summary>
        /// Initializes a new instance of <see cref="PrinterDevice"/>
        /// </summary>
        public PrinterDevice(IActorRef monitor)
        {
            _monitor = monitor;
            _protocol = Context.ActorOf(PrinterProtocol.Props(Self), "Protocol");

            Disconnected();
        }

        /// <summary>
        /// Configures the actor to the disconnected state
        /// </summary>
        private void Disconnected()
        {
            Receive<ConnectDevice>(ConnectToProtocolAndTransport);
        }

        /// <summary>
        /// Configures the actor to the connecting state
        /// </summary>
        private void Connecting()
        {
            Receive<ProtocolConnected>(_ =>
            {
                Become(Connected);
                Stash.UnstashAll();
            });

            ReceiveAny(_ => Stash.Stash());
        }

        /// <summary>
        /// Configures the actor to the connected state
        /// </summary>
        private void Connected()
        {
            // NOTE: Automatically start reporting temperatures upon connection.
            // This assumes that the printer supports it, so I might need to check for the feature toggle.
            _protocol.Tell(new PrinterCommand(null, "M105"));

            Receive<TemperatureReported>(HandleTemperatureReport);
            Receive<DisconnectDevice>(DisconnectFromProtocol);
        }

        /// <summary>
        /// Configures the actor to the disconnecting state
        /// </summary>
        private void Disconnecting()
        {
            Receive<ProtocolDisconnected>(_ => Become(Disconnected));
            ReceiveAny(_ => { });
        }

        /// <summary>
        /// Wires up the protocol and transport layers for the printer.
        /// </summary>
        /// <param name="msg">Command to handle</param>
        private void ConnectToProtocolAndTransport(ConnectDevice msg)
        {
            var transport = Context.ActorOf(SerialPrinterTransport.Props(
                msg.PortName, msg.BaudRate, _protocol));

            _protocol.Tell(new ConnectProtocol(transport));

            Become(Connecting);
        }

        /// <summary>
        /// Disconnects the printer from the protocol layer
        /// </summary>
        /// <param name="_">Command to handle</param>
        private void DisconnectFromProtocol(DisconnectDevice _)
        {
            _protocol.Tell(DisconnectProtocol.Instance);
            Become(Disconnecting);
        }

        /// <summary>
        /// Handles temperature reports from the protocol layer
        /// </summary>
        /// <param name="msg">Message containing temperature measurements</param>
        private void HandleTemperatureReport(TemperatureReported msg)
        {
            _monitor.Tell(msg);
        }

        public IStash Stash { get; set; }
    }
}
