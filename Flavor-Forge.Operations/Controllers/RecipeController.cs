using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace Flavor_Forge.Operations.Controllers
{
    public class RecipeController : Controller
    {

        public IActionResult AddRecipes()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}
