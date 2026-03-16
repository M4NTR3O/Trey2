namespace Trey2.ViewModel
{
    public class Paces
    {
        public float timestamp { get; set; }
        public float? pace { get; set; }

        public Paces(float timestamp, float? pace)
        {
            this.pace = pace;
            this.timestamp = timestamp;
        }
    }
}
