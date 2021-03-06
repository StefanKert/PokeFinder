﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PokeFinder.Misc;
using PokeFinder.Models;

namespace PokeFinder.Services
{
    public class PokemonService: IPokemonService
    {
        private static HttpClient _httpClientInstance;
        public static HttpClient HttpClient => _httpClientInstance ?? (_httpClientInstance = InitClient());

        const string _CACHE_URL = "https://cache.fastpokemap.se/?lat={0}&lng={1}";
        const string _API_URL = "https://api.fastpokemap.se/?lat={0}&lng={1}";

        public async Task<IEnumerable<Pokemon>> ExecuteApiRequest(string latitude, string longitude) {
            try {
                var url = string.Format(_API_URL, latitude, longitude);
                var response = await GetResponseFromUrl(url);
                var stringResult = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<Models.Api.RootObject>(stringResult);
                if (obj?.result != null && obj.result.Count > 0)
                    return obj.result.Select(x => x.ToPokemon());
                return new List<Pokemon>();
            }
            catch (Exception ex) {
                OnException?.Invoke(ex);
                return new List<Pokemon>();
            }
        }

        public async Task<IEnumerable<Pokemon>> ExecuteCacheRequest(string latitude, string longitude) {
            try {
                var url = string.Format(_CACHE_URL, latitude, longitude);
                var response = await GetResponseFromUrl(url);
                var stringResult = await response.Content.ReadAsUnzippedStringAsync();
                var obj = JsonConvert.DeserializeObject<Models.Cache.RootObject[]>(stringResult);
                if (obj != null && obj.Length > 0)
                    return obj.Select(x => x.ToPokemon());
                return new List<Pokemon>();
            }
            catch (Exception ex) {
                OnException?.Invoke(ex);
                return new List<Pokemon>();
            }
        }

        private async Task<HttpResponseMessage> GetResponseFromUrl(string url) {
            return await HttpClient.GetAsync(url);
        }

        private static HttpClient InitClient() {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Origin", "https://fastpokemap.se");
            client.DefaultRequestHeaders.Referrer = new Uri("https://fastpokemap.se");
            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            client.Timeout = TimeSpan.FromMinutes(2);
            return client;
        }


        public event Action<Exception> OnException;
    }
}
