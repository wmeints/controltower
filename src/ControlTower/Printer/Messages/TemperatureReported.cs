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
        /// <param name="bedTemperature">Bed temperature reading</param>
        /// <param name="hotEndTemperature">Hot-end temperature reading</param>
        public TemperatureReported(TemperatureReading bedTemperature, TemperatureReading hotEndTemperature)
        {
            BedTemperature = bedTemperature;
            HotEndTemperature = hotEndTemperature;
        }

        /// <summary>
        ///     Gets the temperature of the bed
        /// </summary>
        public TemperatureReading BedTemperature { get; }

        /// <summary>
        ///     Gets the temperature of the hot-end
        /// </summary>
        public TemperatureReading HotEndTemperature { get; }
    }
}