using System.Collections.Generic;
using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    ///     Records commands that were sent to the transport layer so we can replay them.
    ///     When the printer sends a <c>RSND</c> reply, we'll ask this actor to resend the commands
    ///     from the line number sent back by the printer. The flight recorder will keep on resending
    ///     commands until it's done resending.
    /// </summary>
    public class FlightRecorder : ReceiveActor
    {
        private readonly IActorRef _protocol;
        private readonly Dictionary<int, PrinterCommand> _recordedCommands;
        private int _lineNumber;

        /// <summary>
        ///     Initializes a new instance <see cref="FlightRecorder" />
        /// </summary>
        /// <param name="protocol"></param>
        public FlightRecorder(IActorRef protocol)
        {
            _recordedCommands = new Dictionary<int, PrinterCommand>();
            _protocol = protocol;

            Receive<ReplayCommands>(ReplayCommands);
            Receive<PrinterCommand>(RecordPrinterCommand);
        }

        /// <summary>
        ///     Replays commands from the specified line number
        /// </summary>
        /// <param name="msg">Message containing the line number information to replay from</param>
        private void ReplayCommands(ReplayCommands msg)
        {
            var command = _recordedCommands[msg.LineNumber];
            _protocol.Tell(new ResendCommand(command));

            // Keep replaying until all commands have been resend.
            if (_lineNumber > msg.LineNumber) Self.Tell(new ReplayCommands(msg.LineNumber + 1));
        }

        /// <summary>
        ///     Records a printer command for replay. We're only recording commands that have a line number
        ///     associated with them. All other commands don't have a checksum associated and therefore can't be replayed.
        /// </summary>
        /// <param name="msg"></param>
        private void RecordPrinterCommand(PrinterCommand msg)
        {
            if (msg.LineNumber != null)
            {
                _lineNumber = msg.LineNumber.Value;
                _recordedCommands[msg.LineNumber.Value] = msg;
            }
        }

        /// <summary>
        ///     Creates the properties for the actor
        /// </summary>
        /// <param name="protocol">Protocol to use</param>
        /// <returns></returns>
        public static Props Props(IActorRef protocol)
        {
            return new Props(typeof(FlightRecorder), new object[] {protocol});
        }
    }
}