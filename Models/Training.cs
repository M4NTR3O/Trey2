using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class Training: BaseModel
    {
        [Required]
        required public ulong sportsman_id { get; set; }
        [Required]
        required public byte sport_id { get; set; }
        [Required]
        required public byte type_training_id { get; set; }
        public string? name_training { get; set; }
        [Required]
        required public DateTime time_start { get; set; }
        [Required]
        required public DateTime local_timestamp { get; set; }
        [Required]
        required public float duration { get; set; }
        public float? distance { get; set; }
        public byte? avg_heart_rate { get; set; }
        public byte? min_heart_rate { get; set; }
        public byte? max_heart_rate { get; set; }
        public float? avg_speed { get; set; }
        public float? max_speed { get; set; }
        public ushort? avg_running_cadence { get; set; }
        public ushort? max_running_cadence { get; set; }
        public ushort? avg_power { get; set; }
        public ushort? max_power { get; set; }
        public ushort? total_descent { get; set; }
        public ushort? total_ascent { get; set; }
        public float? start_position_lat { get; set; }
        public float? start_position_long { get; set; }
        public short? max_altitude { get; set; }
        public ushort? calories { get; set; }
        [Required]
        required public bool is_gps { get; set; }
        public byte? stress_score { get; set; }
        public byte? feeling_score { get; set; }
        public string? comment_sportsman { get; set; }
        public ulong? coach_creator {get; set;}
        public string? comment_coach { get; set; }
        public byte? status_training { get; set; }
        public List<TrainingLap>? laps { get; set; }
        public List<Trackpoint>? trackpoints { get; set; }

    }
}
