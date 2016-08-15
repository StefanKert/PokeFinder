using System;
using System.Collections.Generic;

namespace PokeFinder.Models.Api
{
    public class Pokemon
    {
        public string SpawnId { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Result
    {
        public string spawn_point_id { get; set; }
        public string encounter_id { get; set; }
        public string pokemon_id { get; set; }
        public string expiration_timestamp_ms { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

        public Pokemon ToPokemon() {
            return new Pokemon {
                SpawnId = spawn_point_id,
                Name = pokemon_id,
                Id = PokemonList.GetPokemonIdForNameDictionary()[pokemon_id],
                ExpiresAt = new DateTime(),
                Latitude = latitude,
                Longitude = longitude
            };
        }
    }

    public class RootObject
    {
        public List<Result> result { get; set; }
    }
}
