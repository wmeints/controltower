namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is send to the flight recorder to start replaying commands
    /// </summary>
    public class ReplayCommands
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ReplayCommands"/>
        /// </summary>
        /// <param name="lineNumber">Line number to start the replay from</param>
        public ReplayCommands(int lineNumber)
        {
            LineNumber = lineNumber;
        }


        /// <summary>
        /// Gets the line number to start replaying from
        /// </summary>
        public int LineNumber { get; }
    }
}