using MessagePack;
using Trey2.Models;
using Trey2.ViewModels;

namespace Trey2.ViewModel
{
    [MessagePackObject]
    public class TrainingGraphData
    {
        [Key("time_start")]
        public string time_start => TimeStartDt.ToString("o"); // ISO 8601
        [IgnoreMember]
        public DateTime TimeStartDt { get; set; }
        [Key("trackpoints")]
        public List<TrackpointGraph>? trackpoints { get; set; }
        [Key("is_gps")]
        public bool is_gps { get; set; }
    }
}
