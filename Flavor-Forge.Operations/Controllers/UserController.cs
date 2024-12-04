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
            // Use cookies
            string? userIdCookie = _cookieServices.GetCookie("UserId");

            // Check if user is logged in
            if (userIdCookie == null)
            {
                // If no user is logged in send to login page
                return RedirectToAction("Login", "Auth");
            }

            int userId = int.Parse(userIdCookie);
            var user = _userServices.GetUser(userId);

            // If user does not exist than send to login page too
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get user's recipes
            var recipes = _recipeServices.GetRecipesByUserId(userId);

            // Send all required information to View
            ViewBag.Username = user.Username;
            ViewBag.Image = user.Image;
            ViewBag.Age = user.Age;
            ViewBag.Bio = user.Bio;
            ViewBag.UserId = user.UserId;

            // Return View
            return View(recipes);
        }

        public IActionResult Settings()
        {
            // Use cookies
            string? userIdCookie = _cookieServices.GetCookie("UserId");

            // Check if user is logged in
            if (userIdCookie == null)
            {
                // If no user is logged in send to login page
                return RedirectToAction("Login", "Auth");
            }

            int userId = int.Parse(userIdCookie);
            var user = _userServices.GetUser(userId);

            // If user does not exist than send to login page too
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get user's recipes
            var recipes = _recipeServices.GetRecipesByUserId(userId);

            // Send all required information to View (notice it is not all the same as the profile View)
            ViewBag.Username = user.Username;
            ViewBag.UserId = user.UserId;
            ViewBag.Image = user.Image;

            // Return View
            return View(recipes);
        }

        [HttpPost]
        public IActionResult UpdateUsername(int userId, string newUsername)
        {
            // Checker for username (make sure not blank)
            if (string.IsNullOrWhiteSpace(newUsername)) return BadRequest("Username cannot be empty.");

            // Checker for username (make sure user exists)
            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            // Delete cookies related to old username
            _cookieServices.DeleteCookie("UserId");
            _cookieServices.DeleteCookie("Username");

            // Set new username in database
            user.Username = newUsername;
            _userServices.UpdateUser(user);

            // Create new cookies for new username
            _cookieServices.SetCookie("UserId", user.UserId.ToString());
            _cookieServices.SetCookie("Username", user.Username.ToString());

            // Return View for Redirection
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult UpdatePassword(int userId, string newPassword)
        {
            // Checker for password (make sure not blank)
            if (string.IsNullOrWhiteSpace(newPassword)) return BadRequest("Password cannot be empty.");

            // Checker for password (make sure user exists)
            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            // Hash new password
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPassword));
            
            // Set new password in database
            user.Password = Convert.ToBase64String(bytes);
            _userServices.UpdateUser(user);

            // Return View for Redirection
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

            // Redirect to Home Page
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

            // Image checker
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

            // Return View for Redirection
            return RedirectToAction("Profile");
        }



        [HttpPost]
        public IActionResult UpdateAge(int userId, int newAge)
        {
            // Checker for Age
            if (newAge <= 0) return BadRequest("Age must be greater than 0.");

            // Checker for username (make sure user exists)
            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            // Set new age in database
            user.Age = newAge;
            _userServices.UpdateUser(user);

            // Return View for Redirection
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult UpdateBio(int userId, string newBio)
        {
            // Checker for username (make sure user exists)
            var user = _userServices.GetUser(userId);
            if (user == null) return NotFound("User not found.");

            // Set new bio in database
            user.Bio = newBio;
            _userServices.UpdateUser(user);

            // Return View for Redirection
            return RedirectToAction("Profile");
        }
    }
}
