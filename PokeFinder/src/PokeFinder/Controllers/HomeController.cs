using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PokeFinder.Services;

namespace PokeFinder.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}