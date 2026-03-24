using MessagePack;
using System.Collections.Generic;
using Trey2.Models;

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
        public static List<TrackpointGraph> TrackpointToTrackpointGraph(List<Trackpoint>? tps)
        {
            List<TrackpointGraph>? pts = new List<TrackpointGraph>();
            if (tps is not null){
                foreach (var tp in tps)
                {
                    TrackpointGraph pt = new TrackpointGraph
                    {
                        TimeStartDt = tp.duration_point,
                        heart_rate = tp.heart_rate,
                        latitude = tp.latitude,
                        longitude = tp.longitude,
                        altitude = tp.altitude,
                        speed = tp.speed,
                        distance = tp.distance,
                        power = tp.power,
                        cadence = tp.cadence
                    };
                    pts.Add(pt);
                }
            }
            return pts;
        }
    }
}
