namespace Trey2.ViewModel
{
    public class HeartRates
    {
        public float timestamp { get; set; }
        public byte? heart_rate { get; set; }

        public HeartRates(float timestamp, byte? heart_rate)
        {
            this.heart_rate = heart_rate;
            this.timestamp = timestamp;
        }
    }
}
