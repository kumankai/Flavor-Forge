using Microsoft.AspNetCore.Mvc;
using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Flavor_Forge.Operations.Services.Service;

namespace Flavor_Forge.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        private readonly IRecipeServices _recipeServices;
        private readonly ICookiesServices _cookieServices;

        public UserController(IUserServices userServices, IRecipeServices recipeServices, ICookiesServices cookiesServices)
        {
            _userServices = userServices;
            _recipeServices = recipeServices;
            _cookieServices = cookiesServices;
        }

        public IActionResult Profile()
        {
            string? userIdCookie = _cookieServices.GetCookie("UserId");

            // Check if user is logged in
            if (userIdCookie == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = int.Parse(userIdCookie);
            var user = _userServices.GetUser(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get user's recipes
            var recipes = _recipeServices.GetRecipesByUserId(userId);

            ViewBag.Username = user.Username;
            return View(recipes);
        }
    }
}
