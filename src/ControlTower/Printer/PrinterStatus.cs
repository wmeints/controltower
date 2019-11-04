using System;

namespace ControlTower.Printer
{
    /// <summary>
    /// Subscribe to the events on the singleton instance of this class to get information about the printer status.
    /// For example, you can get temperature readings and connection status information through this class.
    /// You can also get information about the currently active job.
    /// </summary>
    public class PrinterStatus
    {
        private TemperatureReading _bedTemperature;
        private bool _connected;
        private TemperatureReading _hotEndTemperature;

        /// <summary>
        /// Gets the job status information
        /// </summary>
        public PrintJobStatus Job { get; } = new PrintJobStatus();

        /// <summary>
        /// Gets or sets the temperature reading for the bed
        /// </summary>
        public TemperatureReading BedTemperature
        {
            get => _bedTemperature;
            set
            {
                _bedTemperature = value;
                PrinterStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the temperature reading for the hot-end
        /// </summary>
        public TemperatureReading HotEndTemperature
        {
            get => _hotEndTemperature;
            set
            {
                _hotEndTemperature = value;
                PrinterStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets whether the printer is connected
        /// </summary>
        /// <value></value>
        public bool Connected
        {
            get => _connected;
            set
            {
                _connected = value;
                PrinterStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets fired when the printer status changes
        /// </summary>
        public event EventHandler PrinterStatusChanged;
    }
}