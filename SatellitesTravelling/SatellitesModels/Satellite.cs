using Satellites;
using Satellites.SatellitesModels;

namespace SatellitesTravelling.SatellitesModels
{
    public class Satellite : Localisation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Altitude { get; set; }
    }
}