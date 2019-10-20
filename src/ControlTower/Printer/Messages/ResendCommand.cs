namespace ControlTower.Printer.Messages
{
    public class ResendCommand
    {
        public ResendCommand(PrinterCommand command)
        {
            Command = command;
        }

        public PrinterCommand Command { get; }
    }
}