using System;
using System.IO;
using Newtonsoft.Json;
using PokeFinder.Models.Api;
using System.Windows.Media.Imaging;

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

        [JsonIgnore]
        public BitmapFrame PictureStream {
            get {
                var png = PokemonList.GetPngForPokemonId(Id);
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(png))) {
                    return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }
        }
    }
}