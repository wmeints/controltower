using System;
using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    ///     Implementation of the printer's application layer.
    /// </summary>
    public class PrinterDevice : ReceiveActor, IWithUnboundedStash
    {
        private readonly IActorRef _monitor;
        private readonly IActorRef _protocol;

        private IActorRef _currentJob;

        /// <summary>
        ///     Initializes a new instance of <see cref="PrinterDevice" />
        /// </summary>
        public PrinterDevice(IActorRef monitor)
        {
            _monitor = monitor;
            _protocol = Context.ActorOf(PrinterProtocol.Props(Self), "protocol");

            Disconnected();
        }

        /// <summary>
        /// Creates the actor properties for the printer
        /// </summary>
        /// <param name="monitor">Monitor to use</param>
        /// <returns>Returns the actor properties</returns>
        public static Akka.Actor.Props Props(IActorRef monitor)
        {
            return new Props(typeof(PrinterDevice), new object[] { monitor });
        }

        public IStash Stash { get; set; }

        /// <summary>
        ///     Configures the actor to the disconnected state
        /// </summary>
        private void Disconnected()
        {
            Receive<ConnectDevice>(ConnectToProtocolAndTransport);
        }

        /// <summary>
        ///     Configures the actor to the connecting state
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
        ///     Configures the actor to the connected state
        /// </summary>
        private void Connected()
        {
            // Schedule the M105 printer command to retrieve temperature readings.
            // This is the best way to do it, since not all printers support the M155 command.
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero, TimeSpan.FromSeconds(5),
                _protocol, new PrinterCommand(null, "M105"), Self);

            _monitor.Tell(DeviceConnected.Instance);

            Receive<TemperatureReported>(HandleTemperatureReport);
            Receive<DisconnectDevice>(DisconnectFromProtocol);

            Receive<StartPrintJob>(StartNewPrintJob);
        }

        /// <summary>
        ///     Configures the actor to the disconnecting state
        /// </summary>
        private void Disconnecting()
        {
            Receive<ProtocolDisconnected>(_ =>
            {
                _monitor.Tell(DeviceDisconnected.Instance);
                Become(Disconnected);
            });

            ReceiveAny(_ => { });
        }

        /// <summary>
        ///     Configures the actor to the printing state
        /// </summary>
        private void Printing()
        {
            Receive<PrinterCommandProcessed>(msg => _currentJob.Tell(msg));
            Receive<PrinterCommand>(cmd => _protocol.Tell(cmd));

            Receive<TemperatureReported>(HandleTemperatureReport);
            Receive<DisconnectDevice>(DisconnectFromProtocol);
            Receive<PrintJobCompleted>(FinishPrintJob);
        }

        /// <summary>
        ///     Starts a new print job
        /// </summary>
        /// <param name="msg"></param>
        private void StartNewPrintJob(StartPrintJob msg)
        {
            _currentJob = Context.ActorOf(PrintJob.Props(msg.Commands, Self, _monitor), "job");
            _currentJob.Tell(StartPrinting.Instance);

            BecomeStacked(Printing);
        }

        /// <summary>
        ///     Completes the current print job
        /// </summary>
        /// <param name="_"></param>
        private void FinishPrintJob(PrintJobCompleted _)
        {
            _currentJob.GracefulStop(TimeSpan.FromMilliseconds(50));

            UnbecomeStacked();
        }

        /// <summary>
        ///     Wires up the protocol and transport layers for the printer.
        /// </summary>
        /// <param name="msg">Command to handle</param>
        private void ConnectToProtocolAndTransport(ConnectDevice msg)
        {
            var transport = Context.ActorOf(SerialPrinterTransport.Props(
                msg.PortName, msg.BaudRate, _protocol), "transport");

            _protocol.Tell(new ConnectProtocol(transport));

            Become(Connecting);
        }

        /// <summary>
        ///     Disconnects the printer from the protocol layer
        /// </summary>
        /// <param name="_">Command to handle</param>
        private void DisconnectFromProtocol(DisconnectDevice _)
        {
            _protocol.Tell(DisconnectProtocol.Instance);
            Become(Disconnecting);
        }

        /// <summary>
        ///     Handles temperature reports from the protocol layer
        /// </summary>
        /// <param name="msg">Message containing temperature measurements</param>
        private void HandleTemperatureReport(TemperatureReported msg)
        {
            _monitor.Tell(msg);
        }
    }
}