using System;
using PokeFinder.Models.Api;

namespace PokeFinder.Models
{
    public enum PokemonType
    {
        Cache,
        Api,
        Nearby
    }

    public class Pokemon
    {
        public PokemonType PokemonType { get; set; }
        public string SpawnId { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}