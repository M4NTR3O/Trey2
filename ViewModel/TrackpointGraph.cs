using MessagePack;

namespace Trey2.ViewModels
{
    [MessagePackObject]
    public class TrackpointGraph
    {
        [Key("duration_point")]
        public string duration_point => TimeStartDt.ToString("o");
        [IgnoreMember]
        public DateTime TimeStartDt { get; set; }

        [Key("heart_rate")]
        public byte? heart_rate { get; set; }
        [Key("latitude")]
        public float? latitude { get; set; }
        [Key("longitude")]
        public float? longitude { get; set; }
        [Key("altitude")]
        public float? altitude { get; set; }
        [Key("speed")]
        public float? speed { get; set; }
        [Key("distance")]
        public float? distance { get; set; }
        [Key("power")]
        public ushort? power { get; set; }
        [Key("cadence")]
        public ushort? cadence { get; set; }
    }
}
