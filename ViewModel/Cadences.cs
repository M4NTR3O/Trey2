namespace Trey2.ViewModel
{
    public class Cadences
    {
        public float timestamp { get; set; }
        public ushort? cadence { get; set; }

        public Cadences(float timestamp, ushort? cadence)
        {
            this.cadence = cadence;
            this.timestamp = timestamp;
        }
    }
}
