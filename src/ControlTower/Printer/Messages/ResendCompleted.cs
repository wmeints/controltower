namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     Gets sent by the flight recorder when it is done resending the commands
    /// </summary>
    public class ResendCompleted
    {
        /// <summary>
        ///     Gets the static instance for the message
        /// </summary>
        public static ResendCompleted Instance { get; } = new ResendCompleted();
    }
}