using Microsoft.AspNetCore.Mvc;
using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Flavor_Forge.Operations.Services.Service;
using Flavor_Forge.Operations.Services.Repository;

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
            ViewBag.Image = user.Image;
            return View(recipes);
        }

        [HttpPost]
        public IActionResult UpdateUsername(int userId, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername)) return BadRequest("Username cannot be empty.");

            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            _cookieServices.DeleteCookie("UserId");
            _cookieServices.DeleteCookie("Username");

            user.Username = newUsername;
            _userServices.UpdateUser(user);

            _cookieServices.SetCookie("UserId", user.UserId.ToString());
            _cookieServices.SetCookie("Username", user.Username.ToString());

            return RedirectToAction("Profile");
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

            return RedirectToAction("Profile");
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


        [HttpPost]
        public async Task<IActionResult> UpdateProfileImage(IFormFile profileImage)
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
                return NotFound("User not found.");
            }

            if (profileImage != null && profileImage.Length > 0)
            {
                // Initialize ImageRepository for image validation and saving
                var imageRepo = new ImageRepository();
                string errorMessage;

                // Validate the image
                if (!imageRepo.ValidateImage(profileImage, out errorMessage))
                {
                    // Handle validation failure
                    ModelState.AddModelError("ProfileImage", errorMessage);
                    return RedirectToAction("Settings"); // Redirect back to settings
                }

                // Save the image and get the relative URL
                var filePath = Path.Combine("wwwroot/recipe-images");
                var imageUrl = await imageRepo.SaveImageAsync(profileImage, filePath);

                // Delete the old image if exists
                if (!string.IsNullOrEmpty(user.Image))
                {
                    imageRepo.DeleteImage(user.Image, filePath);
                }

                // Update the user with the new image URL
                user.Image = imageUrl;
                _userServices.UpdateUser(user);
            }

            return RedirectToAction("Profile"); // Redirect to profile page after updating
        }



        [HttpPost]
        public IActionResult UpdateAge(int userId, int newAge)
        {
            if (newAge <= 0) return BadRequest("Age must be greater than 0.");

            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            user.Age = newAge;
            _userServices.UpdateUser(user);

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult UpdateBio(int userId, string newBio)
        {
            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            user.Bio = newBio;
            _userServices.UpdateUser(user);

            return RedirectToAction("Profile");
        }
    }
}
