namespace ControlTower.Printer.Messages
{
    public class PrinterResponse
    {
        public PrinterResponse(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}