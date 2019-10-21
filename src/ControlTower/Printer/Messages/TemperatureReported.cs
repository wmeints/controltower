namespace ControlTower.Printer.Messages
{
    /// <summary>
    ///     Is sent by the protocol layer when it receives temperature readings
    /// </summary>
    public class TemperatureReported
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="TemperatureReported" />
        /// </summary>
        /// <param name="ambientTemperature">Ambient temperature reading</param>
        /// <param name="bedTemperature">Bed temperature reading</param>
        /// <param name="hotEndTemperature">Hot-end temperature reading</param>
        public TemperatureReported(float? ambientTemperature, float? bedTemperature, float? hotEndTemperature)
        {
            AmbientTemperature = ambientTemperature;
            BedTemperature = bedTemperature;
            HotEndTemperature = hotEndTemperature;
        }

        /// <summary>
        ///     Gets the ambient temperature
        /// </summary>
        public float? AmbientTemperature { get; }

        /// <summary>
        ///     Gets the temperature of the bed
        /// </summary>
        public float? BedTemperature { get; }

        /// <summary>
        ///     Gets the temperature of the hot-end
        /// </summary>
        public float? HotEndTemperature { get; }
    }
}