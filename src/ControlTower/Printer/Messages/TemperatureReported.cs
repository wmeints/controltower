namespace ControlTower.Printer.Messages
{
    public class TemperatureReported
    {
        public TemperatureReported(float? ambientTemperature, float? bedTemperature, float? hotEndTemperature)
        {
            AmbientTemperature = ambientTemperature;
            BedTemperature = bedTemperature;
            HotEndTemperature = hotEndTemperature;
        }

        public float? AmbientTemperature { get; }
        public float? BedTemperature { get; }
        public float? HotEndTemperature { get; }
    }
}
