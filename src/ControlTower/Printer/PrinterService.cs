using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    /// Implementation of the printer service
    /// </summary>
    public class PrinterService: IPrinterService
    {
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
    }
}
