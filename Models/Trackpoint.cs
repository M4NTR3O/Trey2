using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class Trackpoint
    {
        /// <summary>
        /// Point of time - datetime
        /// </summary>
        [Required]
        required public DateTime duration_point { get; set; }
        /// <summary>
        /// Heart rate - byte (May be null)
        /// </summary>
        public byte? heart_rate { get; set; }
        /// <summary>
        /// Latitude - float (May be null)
        /// </summary>
        public float? latitude { get; set; }
        /// <summary>
        /// Longitude - float (May be null)
        /// </summary>
        public float? longitude { get; set; }
        /// <summary>
        /// Altitude - float (May be null)
        /// </summary>
        public float? altitude { get; set; }
        /// <summary>
        /// Speed - float (May be null)
        /// </summary>
        public float? speed { get; set; }
        /// <summary>
        /// Distance - float (May be null)
        /// </summary>
        public float? distance { get; set; }
        /// <summary>
        /// Power - ushort (May be null)
        /// </summary>
        public ushort? power { get; set; }
        /// <summary>
        /// Cadence - ushort (May be null)
        /// </summary>
        public ushort? cadence { get; set; }
    }
}
