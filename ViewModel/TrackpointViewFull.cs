namespace Trey2.ViewModel
{
    public class TrackpointViewFull
    {
        /// <summary>
        /// Point of time - float
        /// </summary>
        public float duration_point { get; set; }

        /// <summary>
        /// Latitude - float (May be null)
        /// </summary>
        public float? latitude { get; set; }
        /// <summary>
        /// Longitude - float (May be null)
        /// </summary>
        public float? longitude { get; set; }

        public TrackpointViewFull(float duration_point, float? latitude, float? longitude)
        {
            this.duration_point = duration_point;
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
