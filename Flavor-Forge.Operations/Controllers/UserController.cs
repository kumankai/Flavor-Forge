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
        private readonly IImageServices _imageServices;
        public UserController(IUserServices userServices, IRecipeServices recipeServices, ICookiesServices cookiesServices, IImageServices imageServices)
        {
            _userServices = userServices;
            _recipeServices = recipeServices;
            _cookieServices = cookiesServices;
            _imageServices = imageServices;
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
            ViewBag.Image = user.Image;
            ViewBag.Age = user.Age;
            ViewBag.Bio = user.Bio;
            ViewBag.UserId = user.UserId;
            return View(recipes);
        }

        public IActionResult Settings()
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
            ViewBag.UserId = user.UserId;
            return View(recipes);
        }

        [HttpPost]
        public IActionResult UpdateUsername(int userId, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername)) return BadRequest("Username cannot be empty.");

            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            user.Username = newUsername;
            _userServices.UpdateUser(user);

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult UpdatePassword(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword)) return BadRequest("Password cannot be empty.");

            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPassword));
            
            user.Password = Convert.ToBase64String(bytes);
            _userServices.UpdateUser(user);

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            // Validate User
            var user = _userServices.GetUser(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Delete User
            _userServices.DeleteUser(userId);

            // Remove Authentication Cookies
            _cookieServices.DeleteCookie("UserId");
            _cookieServices.DeleteCookie("Username");

            // Redirect to Home Page or Login
            return RedirectToAction("Index", "Home");
        }

    }
}
