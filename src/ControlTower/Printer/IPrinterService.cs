using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Starts a new print job given the contents of a G-Code file
        /// </summary>
        /// <param name="fileStream">Stream containing the G-code file data</param>
        /// <returns></returns>
        Task StartJobAsync(Stream fileStream);
    }
}
