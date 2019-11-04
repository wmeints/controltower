using System;
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
                if (msg.HotEndTemperature != null)
                {
                    status.HotEndTemperature = status.HotEndTemperature != null ?
                        status.HotEndTemperature.Merge(msg.HotEndTemperature) :
                        msg.HotEndTemperature;
                }

                if (msg.BedTemperature != null)
                {
                    status.BedTemperature = status.BedTemperature != null ?
                        status.BedTemperature.Merge(msg.BedTemperature) :
                        msg.BedTemperature;
                }
            });

            Receive<PrintJobStepsCompleted>(msg =>
            {
                status.Job.StepsCompleted = msg.StepsCompleted;
                status.Job.TotalSteps = msg.TotalSteps;
            });

            Receive<PrintJobStatusUpdated>(msg => { status.Job.State = msg.State; });
        }

        /// <summary>
        ///     Creates the actor properties for the printer monitor
        /// </summary>
        /// <returns>Returns the actor properties</returns>
        public static Akka.Actor.Props Props(PrinterStatus status)
        {
            return new Props(typeof(PrinterMonitor), new object[] { status });
        }
    }
}