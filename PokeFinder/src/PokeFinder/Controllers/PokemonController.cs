using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PokeFinder.Services;

namespace PokeFinder.Controllers
{
    [Route("api/[controller]")]
    public class PokemonController : Controller
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet("cache")]
        public async Task<IActionResult> GetPokemonFromCache() {
            return Ok(await _pokemonService.ExecuteCacheRequest("34.00788214539798", "-118.49767684936523"));
        }

        [HttpGet("api")]
        public async Task<IActionResult> GetPokemonFromApi() {
            return Ok(await _pokemonService.ExecuteCacheRequest("34.00788214539798", "-118.49767684936523"));
        }
    }
}
