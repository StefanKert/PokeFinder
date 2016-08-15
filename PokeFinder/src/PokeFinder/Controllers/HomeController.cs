using System.IO;
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

        public async Task<IActionResult> Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchRectangle rectangle)
        {
            var longitude1 = double.Parse(rectangle.PrimaryLatLong.Longitude);
            var latitude1 = double.Parse(rectangle.PrimaryLatLong.Longitude);

            var longitude2 = double.Parse(rectangle.SecondaryLatLong.Longitude);
            var latitude2 = double.Parse(rectangle.SecondaryLatLong.Longitude);

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
                    tasks.Add(_pokemonService.ExecuteCacheRequest(j1, i1));
                    //tasks.Add(_pokemonService.ExecuteApiRequest(j1, i1));
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