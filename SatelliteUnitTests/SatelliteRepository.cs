using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SatellitesTravelling;
using SatellitesTravelling.SatellitesModels;

namespace SatelliteUnitTests
{
    public class SatelliteRepository
    {
        public static List<Satellite> GetSatellites()
        {
            var satellite1 = new Satellite
            {
                Id = 0,
                Name = "CREW - 2",
                Latitude = -38.71,
                Longitude = 40.28,
                Altitude = 431.45
            };

            var satellite2 = new Satellite
            {
                Id = 1,
                Name = "SOYUZ MS-18",
                Latitude = -41.8,
                Longitude = 45.52,
                Altitude = 433.58
            };

            var satellite3 = new Satellite
            {
                Id = 2,
                Name = "SHIYAN 6 03(SY-6 03)",
                Latitude = 14.52,
                Longitude = 69.74,
                Altitude = 1004.29
            };

            var satellite4 = new Satellite
            {
                Id = 3,
                Name = "AC 10 PROBE(JACKIE)",
                Latitude = -14.26,
                Longitude = 99.81,
                Altitude = 467.22
            };

            var satellite5 = new Satellite
            {
                Id = 4,
                Name = "STARLINK-2503",
                Latitude = -38.25,
                Longitude = 152.01,
                Altitude = 362
            };

            return new List<Satellite> { satellite1, satellite2, satellite3, satellite4, satellite5 };
        }
    }
}
