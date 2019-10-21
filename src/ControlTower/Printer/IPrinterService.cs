using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer
{
    /// <summary>
    /// Use this service to communicate with the printer
    /// </summary>
    public interface IPrinterService
    {
        /// <summary>
        /// Connects the printer
        /// </summary>
        /// <param name="portName">Port name to connect to</param>
        /// <param name="baudRate">Baud rate to use</param>
        /// <returns></returns>
        Task ConnectAsync(string portName, int baudRate);

        /// <summary>
        /// Lists the available ports to connect to
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetAvailablePortsAsync();

        /// <summary>
        /// Disconnects the printer
        /// </summary>
        /// <returns></returns>
        Task DisconnectAsync();
    }
}
