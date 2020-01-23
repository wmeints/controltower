using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    /// Defines a printer job
    /// </summary>
    public class PrintJob : ReceiveActor
    {
        private readonly int _totalCommands;
        private readonly Queue<PrinterCommand> _commands;
        private readonly IActorRef _device;
        private readonly IActorRef _statusMonitor;

        private int _completedSteps;

        /// <summary>
        /// Initializes a new instance of <see cref="PrintJob"/>
        /// </summary>
        /// <param name="commands">Commands that are part of the printer job</param>
        /// <param name="device">The printer device</param>
        /// <param name="statusMonitor">The status monitor to use</param>
        public PrintJob(IEnumerable<PrinterCommand> commands, IActorRef device, IActorRef statusMonitor)
        {
            var printerCommands = commands.ToList();

            // Enqueue the print job commands, including a reset line number command.
            // The final command in the job queue is the M400 command, it tells the printer to finish its moves.
            // After that we send another command, M110, to reset the linenumber to zero.
            _commands = new Queue<PrinterCommand>(printerCommands);
            _commands.Enqueue(new PrinterCommand(printerCommands.Count + 1, "M400"));

            _totalCommands = printerCommands.Count + 1;

            _device = device;
            _statusMonitor = statusMonitor;

            NotStarted();
        }

        /// <summary>
        /// Defines the actor properties for the print job
        /// </summary>
        /// <param name="commands">Commands for the job</param>
        /// <param name="device">Device to send the commands to</param>
        /// <param name="printerStatus">Printer status to use for reporting progress</param>
        /// <returns>Returns the created actor props</returns>
        public static Props Props(IEnumerable<PrinterCommand> commands, IActorRef device, IActorRef printerStatus)
        {
            return new Props(typeof(PrintJob), new object[] { commands, device, printerStatus });
        }

        /// <summary>
        /// Configures the receive operations for the not-started state of the job
        /// </summary>
        private void NotStarted()
        {
            _statusMonitor.Tell(new PrintJobStatusUpdated(PrintJobState.Ready));

            Receive<StartPrinting>(cmd =>
            {
                _device.Tell(_commands.Dequeue());

                _statusMonitor.Tell(new PrintJobStatusUpdated(PrintJobState.Running));
                _statusMonitor.Tell(new PrintJobStepsCompleted(0, _totalCommands));

                Become(Running);
            });
        }

        /// <summary>
        /// Configures the receive operations for the running state of the job
        /// </summary>
        private void Running()
        {
            Receive<PrinterCommandProcessed>(_ =>
            {
                _completedSteps++;

                _statusMonitor.Tell(new PrintJobStepsCompleted(_completedSteps, _totalCommands));

                if (_commands.Count > 0)
                {
                    _device.Tell(_commands.Dequeue());
                }
                else
                {
                    _statusMonitor.Tell(new PrintJobStatusUpdated(PrintJobState.Completed));
                    _device.Tell(PrintJobCompleted.Instance);

                    Become(Completed);
                }
            });
        }

        /// <summary>
        /// Configures the receive operations for the completed state of the job
        /// </summary>
        private void Completed()
        {
            // Do nothing, we don't want any more commands at this point.
        }
    }
}
