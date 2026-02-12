using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class TrainingLap
    {
        [Required]
        required public int lap_number { get; set; }
        public string? lap_caption { get; set; }
        public double? distance { get; set; }
        [Required]
        required public double? duration { get; set; }
        [Required]
        required public DateTime timestamp { get; set; }
        [Required]
        required public DateTime start_time { get; set; }
        public byte? avg_heart_rate { get; set; }
        public byte? min_heart_rate { get; set; }
        public byte? max_heart_rate { get; set; }
        public double? avg_speed { get; set; }
        public double? max_speed { get; set; }
        public ushort? avg_running_cadence { get; set; }
        public ushort? max_running_cadence { get; set; }
        public ushort? avg_power { get; set; }
        public ushort? max_power { get; set; }
        public ushort? total_descent { get; set; }
        public ushort? total_ascent { get; set; }
    }
}
