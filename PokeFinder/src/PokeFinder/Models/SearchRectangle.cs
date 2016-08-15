using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeFinder.Models
{
    public class LatLong
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class SearchRectangle
    {
        public LatLong PrimaryLatLong { get; set; }

        public LatLong SecondaryLatLong { get; set; }
    }
}
