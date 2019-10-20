using System;
using System.Linq;
using System.Text.RegularExpressions;
using Akka.Actor;
using ControlTower.Controller;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
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

        public PrinterProtocol(IActorRef printer)
        {
            _printer = printer;

            Disconnected();
        }

        public static Akka.Actor.Props Props(IActorRef printer)
        {
            return new Props(typeof(PrinterProtocol), new object[] { printer });
        }

        public IStash Stash { get; set; }

        private void Disconnected()
        {
            Receive<TransportDisconnected>(_ =>
            {
                _printer.Tell(ProtocolDisconnected.Instance);
            });

            Receive<ConnectProtocol>(ConnectToTransport);
        }

        private void Idle()
        {
            Receive<PrinterCommand>(ProcessIncomingCommand);
            Receive<PrinterResponse>(ProcessIncomingResponse);
            Receive<DisconnectProtocol>(DisconnectFromTransport);
        }

        private void WaitingForResponse()
        {
            Receive<PrinterResponse>(ProcessIncomingResponse);
            Receive<DisconnectProtocol>(DisconnectFromTransport);
            ReceiveAny(cmd => Stash.Stash());
        }

        private void ResendingCommands()
        {
            Receive<PrinterResponse>(ProcessIncomingResponse);
            Receive<ResendCommand>(ProcessResendCommand);
            Receive<ResendCompleted>(CompleteResend);
            Receive<DisconnectProtocol>(DisconnectFromTransport);
            ReceiveAny(_ => Stash.Stash());
        }

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

        private void ConnectToTransport(ConnectProtocol obj)
        {
            _transport = obj.Transport;
            _flightRecorder = Context.ActorOf(FlightRecorder.Props(Self), "flight-recorder");

            _transport.Tell(ConnectTransport.Instance);
            
            Become(Connecting);
        }

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

        private void ProcessResendCommand(ResendCommand msg)
        {
            _transport.Tell(msg.Command);

            _waitingForResponse = true;
            BecomeStacked(WaitingForResponse);
        }

        private void CompleteResend(ResendCompleted obj)
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