using System;
using System.Collections.Generic;
using PokeFinder.Models.Api;

namespace PokeFinder.Models.Cache
{
    public class Lnglat
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class RootObject
    {
        public string _id { get; set; }
        public string pokemon_id { get; set; }
        public ulong encounter_id { get; set; }
        public string spawn_id { get; set; }
        public string expireAt { get; set; }
        public int __v { get; set; }
        public Lnglat lnglat { get; set; }

        public Pokemon ToPokemon()
        {
            return new Pokemon
            {
                SpawnId = spawn_id,
                Name = pokemon_id,
                Id = PokemonList.GetPokemonIdForNameDictionary()[pokemon_id],
                ExpiresAt = DateTime.Parse(expireAt),
                Longitude = lnglat.coordinates[0],
                Latitude = lnglat.coordinates[1]
            };
        }
    }
}