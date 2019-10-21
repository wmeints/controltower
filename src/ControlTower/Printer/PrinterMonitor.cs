using Akka.Actor;
using ControlTower.Printer.Messages;

namespace ControlTower.Printer
{
    /// <summary>
    ///     Handles printer status reports and translates to stuff that Blazor understands.
    /// </summary>
    public class PrinterMonitor : ReceiveActor
    {
        private readonly PrinterStatus _status;

        /// <summary>
        ///     Initializes a new instance of <see cref="PrinterMonitor" />
        /// </summary>
        /// <param name="status">Status object to connect to</param>
        public PrinterMonitor(PrinterStatus status)
        {
            _status = status;

            Receive<DeviceConnected>(_ => status.Connected = true);
            Receive<DeviceDisconnected>(_ => status.Connected = false);

            Receive<TemperatureReported>(msg =>
            {
                status.AmbientTemperature = msg.AmbientTemperature;
                status.BedTemperature = msg.BedTemperature;
                status.HotEndTemperature = msg.HotEndTemperature;
            });
        }
    }
}