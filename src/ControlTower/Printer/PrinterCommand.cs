using System;

namespace ControlTower.Printer
{
    /// <summary>
    /// A printer command represents a singular command given by the application or a single line
    /// from a GCode file. In the case of a GCode file, this command will have a line number and checksum flag
    /// set for the command.
    /// </summary>
    public class PrinterCommand
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PrinterCommand"/>.
        /// </summary>
        /// <param name="text">Text to transmit</param>
        /// <param name="calculateChecksum">Whether a checksum should be calculated.</param>
        /// <param name="lineNumber">The line number of the command in the original file.</param>
        public PrinterCommand(string text, bool calculateChecksum, int lineNumber)
        {
            Text = text;
            CalculateChecksum = calculateChecksum;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Gets the text to transmit
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the flag indicating a checksum should be calculated.
        /// </summary>
        public bool CalculateChecksum { get; set; }

        /// <summary>
        /// Gets the line number in the original file.
        /// </summary>
        public int LineNumber { get; set; }
    }
}