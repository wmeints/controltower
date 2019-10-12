namespace ControlTower.Printer
{
    /// <summary>
    /// Event args for the <see cref="PrinterProtocol.TemperatureReported"/> event.
    /// </summary>
    public class TemperatureReportedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TemperatureReportedEventArgs"/>
        /// </summary>
        /// <param name="ambientTemperature">The ambient temperature reading</param>
        /// <param name="bedTemperature">The bed temperature reading</param>
        /// <param name="hotEndTemperature">The hot-end temperature reading</param>
        public TemperatureReportedEventArgs(float? ambientTemperature, float? bedTemperature, float? hotEndTemperature)
        {
            AmbientTemperature = ambientTemperature;
            BedTemperature = bedTemperature;
            HotEndTemperature = hotEndTemperature;
        }

        /// <summary>
        /// Gets the ambient temperature reading in degrees celcius 
        /// </summary>
        public float? AmbientTemperature { get; }
        
        /// <summary>
        /// Gets the bed temperature reading in degrees celcius 
        /// </summary>
        public float? BedTemperature { get; }
        
        /// <summary>
        /// Gets the hot-end temperature reading in degrees celcius 
        /// </summary>
        public float? HotEndTemperature { get; }
    }
}