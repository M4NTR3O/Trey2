namespace Trey2.Models
{
    public class Trackpoint
    {
        public DateTime? duration_point { get; set; }
        public byte? heart_rate { get; set; }
        public float? latitude { get; set; }
        public float? longitude { get; set; }
        public float? altitude { get; set; }
        public float? speed { get; set; }
        public float? distance { get; set; }
        public ushort? power { get; set; }
        public ushort? cadence { get; set; }
    }
}
