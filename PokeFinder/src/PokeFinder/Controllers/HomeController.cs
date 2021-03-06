﻿using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PokeFinder.Services;
using PokeFinder.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokeFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPokemonService _pokemonService;

        public HomeController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        public async Task<IActionResult> AngularIndex()
        {
            return View();
        }

        public async Task<IActionResult> Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchRectangle rectangle)
        {
            var longitude1 = rectangle.PrimaryLatLong.Longitude;
            var latitude1 = rectangle.PrimaryLatLong.Latitude;

            var longitude2 = rectangle.SecondaryLatLong.Longitude;
            var latitude2 = rectangle.SecondaryLatLong.Latitude;

            double higherLatitude = latitude1 > latitude2 ? latitude1 : latitude2;
            double lowLatitude = latitude1 > latitude2 ? latitude2 : latitude1;
            double higherLongitude = longitude1 > longitude2 ? longitude1 : longitude2;
            double lowerLongitude = longitude1 > longitude2 ? longitude2 : longitude1;

            var tasks = new List<Task<IEnumerable<Pokemon>>>();

            for (double i = lowLatitude; i < higherLatitude; i += 0.0020)
            {
                double j = lowerLongitude;
                do
                {
                    var i1 = i.ToString("G");
                    var j1 = j.ToString("G");
                    tasks.Add(_pokemonService.ExecuteCacheRequest(i1, j1));
                    tasks.Add(_pokemonService.ExecuteApiRequest(i1, j1));
                    j += 0.0020;
                } while (j < higherLongitude);
            }

            var result = await Task.WhenAll(tasks.ToArray());
            var viewModel = new PokemonListViewModel
            {
                SearchRectangle = rectangle,
                Pokemon = result.SelectMany(x => x).ToList()
            };
            return View("PokemonList", viewModel);
        }
    }
}