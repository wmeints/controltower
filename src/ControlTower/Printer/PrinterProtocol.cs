using System;
using System.Linq;
using System.Text.RegularExpressions;
using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    /// Implements the protocol layer for the 3D printer.
    /// </summary>
    public class PrinterProtocol : ReceiveActor, IWithUnboundedStash
    {
        private static readonly Regex ResendPattern = new Regex("N:?|:");
        private static readonly Regex SplitPattern = new Regex("\\s+");
        private static readonly Regex AmbientTemperaturePattern = new Regex(@"T:(?<temp>(\d+)(\.(\d+)))?");
        private static readonly Regex HotEndTemperaturePattern = new Regex(@"E:(?<temp>(\d+)(\.(\d+)))?");
        private static readonly Regex BedTemperaturePattern = new Regex(@"B:(?<temp>(\d+)(\.(\d+)))?");

        private readonly IActorRef _printer;

        private IActorRef _transport;
        private IActorRef _flightRecorder;
        
        private bool _waitingForResponse;

        /// <summary>
        /// Initializes a new instance of <see cref="PrinterProtocol"/>
        /// </summary>
        /// <param name="printer"></param>
        public PrinterProtocol(IActorRef printer)
        {
            _printer = printer;

            Disconnected();
        }

        /// <summary>
        /// Creates the properties for the protocol layer
        /// </summary>
        /// <param name="printer">Printer device to connect the protocol layer</param>
        /// <returns>Returns the actor properties</returns>
        public static Akka.Actor.Props Props(IActorRef printer)
        {
            return new Props(typeof(PrinterProtocol), new object[] { printer });
        }

        /// <summary>
        /// Gets or sets the stash used by the actor
        /// </summary>
        public IStash Stash { get; set; }

        /// <summary>
        /// Configures the actor to the disconnected state
        /// </summary>
        private void Disconnected()
        {
            Receive<TransportDisconnected>(_ =>
            {
                _printer.Tell(ProtocolDisconnected.Instance);
            });

            Receive<ConnectProtocol>(ConnectToTransport);
        }

        /// <summary>
        /// Configures the actor the idle state
        /// </summary>
        private void Idle()
        {
            Receive<PrinterCommand>(ProcessIncomingCommand);
            Receive<PrinterResponse>(ProcessIncomingResponse);
            Receive<DisconnectProtocol>(DisconnectFromTransport);
        }

        /// <summary>
        /// Configures the actor to wait for a printer response
        /// </summary>
        private void WaitingForResponse()
        {
            Receive<PrinterResponse>(ProcessIncomingResponse);
            Receive<DisconnectProtocol>(DisconnectFromTransport);
            ReceiveAny(cmd => Stash.Stash());
        }

        /// <summary>
        /// Configures the actor the the resending state
        /// </summary>
        private void ResendingCommands()
        {
            Receive<PrinterResponse>(ProcessIncomingResponse);
            Receive<ResendCommand>(ProcessResendCommand);
            Receive<ResendCompleted>(CompleteResend);
            Receive<DisconnectProtocol>(DisconnectFromTransport);
            ReceiveAny(_ => Stash.Stash());
        }

        /// <summary>
        /// Configures the actor to the connecting state
        /// </summary>
        private void Connecting()
        {
            Receive<TransportConnected>(_ =>
            {
                _printer.Tell(ProtocolConnected.Instance);

                Stash.UnstashAll();
                Become(Idle);
            });

            ReceiveAny(_ => Stash.Stash());
        }

        /// <summary>
        /// Connects the protocol layer to the transport layer
        /// </summary>
        /// <param name="msg">Message to handle</param>
        private void ConnectToTransport(ConnectProtocol msg)
        {
            _transport = msg.Transport;
            _flightRecorder = Context.ActorOf(FlightRecorder.Props(Self), "flight-recorder");

            _transport.Tell(ConnectTransport.Instance);
            
            Become(Connecting);
        }

        /// <summary>
        /// Processes the incoming printer data
        /// </summary>
        /// <param name="msg">Message to handle</param>
        private void ProcessIncomingResponse(PrinterResponse msg)
        {
            if (msg.Text.Contains("T:"))
            {
                var (ambientTemperature, bedTemperature, hotEndTemperature) = ParseTemperatureReadings(msg.Text);
                _printer.Tell(new TemperatureReported(ambientTemperature, bedTemperature, hotEndTemperature));
            }

            if (msg.Text.StartsWith("ok") || msg.Text.StartsWith("start") || msg.Text.StartsWith("Gbrl "))
            {
                if (_waitingForResponse)
                {
                    _waitingForResponse = false;

                    _printer.Tell(PrinterCommandProcessed.Instance);

                    Stash.UnstashAll();
                    UnbecomeStacked();
                }
            }

            if (msg.Text.StartsWith("resend") || msg.Text.StartsWith("rs"))
            {
                var line = ResendPattern.Replace(msg.Text, " ");
                var words = SplitPattern.Split(line).ToList();

                while (words.Count > 0)
                {
                    var word = words[0];
                    words.RemoveAt(0);

                    if (int.TryParse(word, out var resendIndex))
                    {
                        _flightRecorder.Tell(new ReplayCommands(resendIndex));
                        BecomeStacked(ResendingCommands);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes an incoming printer command
        /// </summary>
        /// <param name="msg">Message to process</param>
        private void ProcessIncomingCommand(PrinterCommand msg)
        {
            if (msg.LineNumber != null)
            {
                _flightRecorder.Tell(msg);
            }

            _transport.Tell(msg);

            _waitingForResponse = true;
            BecomeStacked(WaitingForResponse);
        }

        /// <summary>
        /// Processes the resend command coming from the flight recorder.
        /// </summary>
        /// <param name="msg">Message to resend</param>
        private void ProcessResendCommand(ResendCommand msg)
        {
            _transport.Tell(msg.Command);

            _waitingForResponse = true;
            BecomeStacked(WaitingForResponse);
        }

        /// <summary>
        /// Completes the resend procedure
        /// </summary>
        /// <param name="_">Message to handle</param>
        private void CompleteResend(ResendCompleted _)
        {
            UnbecomeStacked();
        }

        /// <summary>
        /// Disconnects from the transport layer
        /// </summary>
        /// <param name="obj">Incoming message</param>
        private void DisconnectFromTransport(DisconnectProtocol obj)
        {
            _transport.Tell(DisconnectTransport.Instance, Self);
            _flightRecorder.GracefulStop(TimeSpan.FromMilliseconds(10));

            Become(Disconnected);
        }

        /// <summary>
        /// Parses temperature reading from text received from the transport layer.
        /// </summary>
        /// <param name="text">Text received from the transport layer</param>
        /// <returns>Returns a tuple containing the ambient, bed, and finally, the hotend temperature</returns>
        private (float?, float?, float?) ParseTemperatureReadings(string text)
        {
            float? ParseTemperatureReading(string raw, Regex pattern)
            {
                if (pattern.Match(raw) is { Value: var value })
                {
                    return Single.Parse(value);
                }

                return null;
            }

            return (
                ParseTemperatureReading(text, AmbientTemperaturePattern),
                ParseTemperatureReading(text, BedTemperaturePattern),
                ParseTemperatureReading(text, HotEndTemperaturePattern)
            );
        }
    }

}