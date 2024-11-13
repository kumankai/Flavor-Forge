using Microsoft.AspNetCore.Mvc;

namespace Flavor_Forge.Operations.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
