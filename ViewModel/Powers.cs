namespace Trey2.ViewModel
{
    public class Powers
    {
        public float timestamp { get; set; }
        public ushort? power { get; set; }

        public Powers(float timestamp, ushort?  power)
        {
            this.power = power;
            this.timestamp = timestamp;
        }
    }
}
