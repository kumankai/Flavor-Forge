using Microsoft.AspNetCore.Mvc;

namespace Flavor_Forge.Operations.Controllers
{
    public class RecipeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
