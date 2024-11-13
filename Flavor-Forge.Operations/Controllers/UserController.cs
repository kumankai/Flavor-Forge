using Microsoft.AspNetCore.Mvc;

namespace Flavor_Forge.Operations.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View();
        }
    }
}
