using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PokeFinder.Controllers
{
    namespace WebApplication1.Controllers
    {
        public class PartialController : Controller
        {
            public IActionResult Message() => PartialView();

            public IActionResult Numbers() => PartialView();
        }
    }
}
