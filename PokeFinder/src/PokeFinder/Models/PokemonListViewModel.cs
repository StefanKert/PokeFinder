using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeFinder.Models
{
    public class PokemonListViewModel
    {
        public SearchRectangle SearchRectangle { get; set; }
        public List<Pokemon> Pokemon { get; set; }
    }
}
