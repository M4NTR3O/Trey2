namespace Trey2.ViewModel
{
    public class Altitudes
    {
        public float timestamp { get; set; }
        public float? altitude { get; set; }

        public Altitudes(float timestamp, float? altitude)
        {
            this.altitude = altitude;
            this.timestamp = timestamp;
        }
    }
}
