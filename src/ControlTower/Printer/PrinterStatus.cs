using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTower.Printer
{
    public class PrinterStatus
    {
        private float? _ambientTemperature;
        private float? _bedTemperature;
        private float? _hotEndTemperature;
        private bool _connected;

        public event EventHandler PrinterStatusChanged;

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
    }
}
