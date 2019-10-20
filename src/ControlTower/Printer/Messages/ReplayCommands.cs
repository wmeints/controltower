namespace ControlTower.Printer.Messages
{
    public class ReplayCommands
    {
        public ReplayCommands(int lineNumber)
        {
            LineNumber = lineNumber;
        }

        public int LineNumber { get; }
    }
}