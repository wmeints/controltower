using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    /// Implementation of the printer service
    /// </summary>
    public class PrinterService : IPrinterService
    {
        private static readonly Regex LinePattern = new Regex(@"^(?<text>[^;]*)(\s*;.*)?$");

        private readonly IActorRef _printer;

        /// <summary>
        /// Initializes a new instance of <see cref="PrinterService"/>
        /// </summary>
        /// <param name="printer">Printer actor to use</param>
        public PrinterService(IActorRef printer)
        {
            _printer = printer;
        }

        /// <inheritdoc />
        public Task ConnectAsync(string portName, int baudRate)
        {
            _printer.Tell(new ConnectDevice(portName, baudRate));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DisconnectAsync()
        {
            _printer.Tell(DisconnectDevice.Instance);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetAvailablePortsAsync()
        {
            return Task.FromResult((IEnumerable<string>)SerialPort.GetPortNames());
        }

        /// <inheritdoc />
        public Task StartJobAsync(Stream fileStream)
        {
            IEnumerable<PrinterCommand> commands = ParseInputFile(fileStream).ToList();
            
            _printer.Tell(new StartPrintJob(commands));

            return Task.CompletedTask;
        }

        private IEnumerable<PrinterCommand> ParseInputFile(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            var lineNumber = 1;

            while (!reader.EndOfStream)
            {
                var rawLine = reader.ReadLine();
                    
                if (rawLine != null)
                {
                    var lineMatch = LinePattern.Match(rawLine);

                    if (lineMatch.Success)
                    {
                        var lineText = lineMatch.Groups["text"].Value.Trim();
                        
                        // Even if the line was initially non-empty, we could still end up with
                        // an empty line, because it was only a comment. Skip these as well.
                        if (!string.IsNullOrEmpty(lineText))
                        {
                            yield return new PrinterCommand(lineNumber, lineText);
                        }
                    }
                }

                lineNumber++;
            }
        }
    }
}
