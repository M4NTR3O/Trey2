using System.ComponentModel.DataAnnotations;

namespace Trey2.ViewModel
{
    public class TrackpointView
    {
        /// <summary>
        /// Point of time - datetime
        /// </summary>
        public DateTime duration_point { get; set; }

        /// <summary>
        /// Latitude - float (May be null)
        /// </summary>
        public float? latitude { get; set; }
        /// <summary>
        /// Longitude - float (May be null)
        /// </summary>
        public float? longitude { get; set; }
    }
}
