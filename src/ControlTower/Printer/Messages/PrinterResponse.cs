namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     This message is sent by the transport layer when it receives a response from the printer
    /// </summary>
    public class PrinterResponse
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="PrinterResponse" />
        /// </summary>
        /// <param name="text"></param>
        public PrinterResponse(string text)
        {
            Text = text;
        }

        /// <summary>
        ///     Gets the text sent by the printer
        /// </summary>
        public string Text { get; }
    }
}