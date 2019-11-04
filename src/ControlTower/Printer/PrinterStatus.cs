using System;

namespace ControlTower.Printer
{
    public class PrinterStatus
    {
        private float? _ambientTemperature;
        private float? _bedTemperature;
        private bool _connected;
        private float? _hotEndTemperature;

        /// <summary>
        /// Gets the job status information
        /// </summary>
        public PrintJobStatus Job { get; } = new PrintJobStatus();

        public float? AmbientTemperature
        {
            get => _ambientTemperature;
            set
            {
                _ambientTemperature = value;
                PrinterStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public float? BedTemperature
        {
            get => _bedTemperature;
            set
            {
                _bedTemperature = value;
                PrinterStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public float? HotEndTemperature
        {
            get => _hotEndTemperature;
            set
            {
                _hotEndTemperature = value;
                PrinterStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool Connected
        {
            get => _connected;
            set
            {
                _connected = value;
                PrinterStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler PrinterStatusChanged;
    }
}