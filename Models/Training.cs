namespace Trey2.Models
{
    public class Training: BaseModel
    {
        public int? sportsmen_id { get; set; }
        public string? sport_id { get; set; }
        public int? type_training_id { get; set; }
        public string? name { get; set; }
        public DateTime? time_start { get; set; }
        public DateTime? local_timestamp { get; set; }
        public double? duration { get; set; }
        public double? distance { get; set; }
        public int? avg_heart_rate { get; set; }
        public double? avg_speed { get; set; }
        public int? min_heart_rate { get; set; }
        public int? max_heart_rate { get; set; }
        public double? max_speed { get; set; }
        public double? avg_running_cadence { get; set; }
        public double? max_running_cadence { get; set; }
        public int? avg_power { get; set; }
        public int? max_power { get; set; }
        public int? total_descent { get; set; }
        public int? total_ascent { get; set; }
        public long? start_position_lat { get; set; }
        public long? start_position_long { get; set; }
        public int? max_altitude { get; set; }
        public int? calories { get; set; }
        public bool is_gps { get; set; }
        public List<TrainingLap>? laps { get; set; }
        public List<Training_GPS_HR>? training_gps_hr_points { get; set; }
        public List<TrainingHR>? training_hr_points { get; set; }

    }
}
