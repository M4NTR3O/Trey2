namespace Trey2.ViewModel
{
    public class GraphModel
    {
        public List<TrackpointFromPython>? trackpoint { get; set; }
        public List<Pauses>? pauses { get; set; }

        public List<TrackpointViewFull>? GetTrackpoints()
        {
            List<TrackpointViewFull>? result = new List<TrackpointViewFull>();
            if (trackpoint is not null)
            {
                foreach (var c in trackpoint)
                {
                    TrackpointViewFull trp = new TrackpointViewFull(c.duration, c.latitude, c.longitude);
                    if (trp is not null)
                    {
                        result.Add(trp);
                    }
                }
            }
            return result;
        }

        public List<HeartRates>? GetHeart_rates()
        {
            List<HeartRates>? result = new List<HeartRates>();
            if (trackpoint is not null)
            {
                foreach (var c in trackpoint)
                {
                    HeartRates trp = new HeartRates(c.duration, c.heart_rate);
                    if (trp is not null)
                    {
                        result.Add(trp);
                    }
                }
            }
            return result;
        }

        public List<Paces>? GetPaces()
        {
            List<Paces>? result = new List<Paces>();
            if (trackpoint is not null)
            {
                foreach (var c in trackpoint)
                {
                    Paces trp = new Paces(c.duration, c.speed);
                    if (trp is not null)
                    {
                        result.Add(trp);
                    }
                }
            }
            return result;
        }

        public List<Altitudes>? GetAltitudes()
        {
            List<Altitudes>? result = new List<Altitudes>();
            if (trackpoint is not null)
            {
                foreach (var c in trackpoint)
                {
                    Altitudes trp = new Altitudes(c.duration, c.altitude);
                    if (trp is not null)
                    {
                        result.Add(trp);
                    }
                }
            }
            return result;
        }

        public List<Powers>? GetPowers()
        {
            List<Powers>? result = new List<Powers>();
            if (trackpoint is not null)
            {
                foreach (var c in trackpoint)
                {
                    Powers trp = new Powers(c.duration, c.power);
                    if (trp is not null)
                    {
                        result.Add(trp);
                    }
                }
            }
            return result;
        }

        public List<Cadences>? GetCadences()
        {
            List<Cadences>? result = new List<Cadences>();
            if (trackpoint is not null)
            {
                foreach (var c in trackpoint)
                {
                    Cadences trp = new Cadences(c.duration, c.cadence);
                    if (trp is not null)
                    {
                        result.Add(trp);
                    }
                }
            }
            return result;
        }
    }
}
