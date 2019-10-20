using System.Collections.Generic;
using System.Net.NetworkInformation;
using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Controller
{
    public class FlightRecorder: ReceiveActor
    {
        private readonly IActorRef _protocol;
        private readonly Dictionary<int, PrinterCommand> _recordedCommands;
        private int _lineNumber;
        
        public FlightRecorder(IActorRef protocol)
        {
            _recordedCommands = new Dictionary<int, PrinterCommand>();
            _protocol = protocol;
            
            Receive<ReplayCommands>(ReplayCommands);
            Receive<PrinterCommand>(RecordPrinterCommand);
        }

        private void ReplayCommands(ReplayCommands obj)
        {
            var command = _recordedCommands[obj.LineNumber];
            _protocol.Tell(new ResendCommand(command));

            // Keep replaying until all commands have been resend.
            if (_lineNumber > obj.LineNumber)
            {
                Self.Tell(new ReplayCommands(obj.LineNumber + 1));
            }
        }
        
        private void RecordPrinterCommand(PrinterCommand obj)
        {
            if (obj.LineNumber != null)
            {
                _lineNumber = obj.LineNumber.Value;
                _recordedCommands[obj.LineNumber.Value] = obj;    
            }
        }

        public static Akka.Actor.Props Props(IActorRef protocol)
        {
            return new Props(typeof(FlightRecorder), new object[] { protocol });
        }
    }
}