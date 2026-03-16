namespace Trey2.ViewModel
{
    public class Pauses
    {
        public float pause_start_relative { get; set; }
        public float pause_duration { get; set; }
        public Pauses(float start, float duration)
        {
            pause_start_relative = start;
            pause_duration = duration;
        }
    }
}
