namespace ControlTower.Printer.Messages
{
    /// <summary>
    /// This message is used to send commands to the printer.
    /// Commands from a job have a line number associated with them.
    /// Other commands don't use this mechanic.
    /// </summary>
    public class PrinterCommand
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PrinterCommand"/>
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="text"></param>
        public PrinterCommand(int? lineNumber, string text)
        {
            LineNumber = lineNumber;
            Text = text;
        }

        /// <summary>
        /// Gets the line number for the command
        /// </summary>
        public int? LineNumber { get; }

        /// <summary>
        /// Gets the command text to send
        /// </summary>
        public string Text { get; }
        
        /// <summary>
        /// Converts the command into its serialized form
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            if (LineNumber != null)
            {
                var prefix = $"N{LineNumber.Value} {Text}";
                var checksum = CalculateChecksum(prefix);

                return $"{prefix}*{checksum}";
            }

            return Text;
        }

        /// <summary>
        /// Calculates the checksum for the command.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int CalculateChecksum(string text)
        {
            int checksum = text[0];

            if (text.Length > 1)
            {
                for (int i = 1; i < text.Length; i++)
                {
                    checksum = checksum ^ text[i];
                }
            }

            return checksum;
        }
    }
}