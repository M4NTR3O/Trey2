using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class TrainingLap
    {
        public int lap_number { get; set; }
        public string? lap_caption { get; set; }
        public double? distance { get; set; }
        public double? duration { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public DateTimeOffset start_time { get; set; }
        public byte? avg_heart_rate { get; set; }
        public byte? min_heart_rate { get; set; }
        public byte? max_heart_rate { get; set; }
        public double? avg_speed { get; set; }
        public double? max_speed { get; set; }
        public double? avg_running_cadence { get; set; }
        public double? max_running_cadence { get; set; }
        public short? avg_power { get; set; }
        public short? max_power { get; set; }
        public short? total_descent { get; set; }
        public short? total_ascent { get; set; }
    }
}
