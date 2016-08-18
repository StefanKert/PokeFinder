using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeFinder.Models
{
    public class LatLong
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class SearchRectangle
    {
        public LatLong PrimaryLatLong { get; set; }

        public LatLong SecondaryLatLong { get; set; }
    }
}
