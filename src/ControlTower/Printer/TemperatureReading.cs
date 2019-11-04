namespace ControlTower.Printer
{
    public class TemperatureReading
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TemperatureReading"/>
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="targetValue">Target value</param>
        public TemperatureReading(float? value, float? targetValue)
        {
            Value = value;
            TargetValue = targetValue;
        }

        /// <summary>
        /// Gets the current value
        /// </summary>
        public float? Value { get; }

        /// <summary>
        /// Gets the target value
        /// </summary>
        public float? TargetValue { get; }

        /// <summary>
        /// Merges two reported readings into a single new one.
        /// </summary>
        /// <param name="reading">Newly recorded reading</param>
        /// <returns>Merged reading</returns>
        public TemperatureReading Merge(TemperatureReading reading)
        {
            return new TemperatureReading(
                reading.Value != null ? reading.Value : Value,
                reading.TargetValue != null ? reading.TargetValue : TargetValue);
        }
    }
}