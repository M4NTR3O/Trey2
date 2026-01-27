using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class TrainingLap
    {
        public int lap_number { get; set; }
        public string? lap_caption { get; set; }
        public double? distance { get; set; }
        public double? duration { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime start_time { get; set; }
        public int? avg_heart_rate { get; set; }
        public int? min_heart_rate { get; set; }
        public int? max_heart_rate { get; set; }
        public double? avg_speed { get; set; }
        public double? max_speed { get; set; }
        public double? avg_running_cadence { get; set; }
        public double? max_running_cadence { get; set; }
        public int? avg_power { get; set; }
        public int? max_power { get; set; }
        public int? total_descent { get; set; }
        public int? total_ascent { get; set; }
    }
}
