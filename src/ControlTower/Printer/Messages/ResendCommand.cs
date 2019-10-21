namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     This message is sent by the flight recorder to the protocol layer to resend a command
    /// </summary>
    public class ResendCommand
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="ResendCommand" />
        /// </summary>
        /// <param name="command">Command to resend</param>
        public ResendCommand(PrinterCommand command)
        {
            Command = command;
        }

        /// <summary>
        ///     Gets the command to replay
        /// </summary>
        public PrinterCommand Command { get; }
    }
}