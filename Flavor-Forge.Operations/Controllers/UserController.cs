using Microsoft.AspNetCore.Mvc;
using Flavor_Forge.Services;
using Flavor_Forge.Entities;

namespace Flavor_Forge.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        private readonly IRecipeServices _recipeServices;
        private readonly IIngredientServices _ingredientServices;

        public UserController(IUserServices userServices, IRecipeServices recipeServices, IIngredientServices ingredientServices)
        {
            _userServices = userServices;
            _recipeServices = recipeServices;
            _ingredientServices = ingredientServices;
        }

        public IActionResult Profile()
        {
            // Check if user is logged in
            if (!Request.Cookies.ContainsKey("UserId"))
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = int.Parse(Request.Cookies["UserId"]);
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
