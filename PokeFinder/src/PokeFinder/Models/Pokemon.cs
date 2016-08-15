using System;

namespace PokeFinder.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public string SpawnId { get; set; }
        public string Name { get; set; }
        public DateTime ExpiresAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}