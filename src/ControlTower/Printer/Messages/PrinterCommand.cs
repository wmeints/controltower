namespace ControlTower.Printer.Messages
{
    public class PrinterCommand
    {
        public PrinterCommand(int? lineNumber, string text)
        {
            LineNumber = lineNumber;
            Text = text;
        }

        public int? LineNumber { get; }
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