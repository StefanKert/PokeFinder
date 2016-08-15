using System.Collections.Generic;
using System.Threading.Tasks;
using PokeFinder.Models.Api;

namespace PokeFinder.Services
{
    public interface IPokemonService
    {
        Task<IEnumerable<Pokemon>> ExecuteCacheRequest(string longitude, string latitude);

        Task<IEnumerable<Pokemon>> ExecuteApiRequest(string latitude, string longitude);
    }
}