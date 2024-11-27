using Microsoft.AspNetCore.Mvc;
using Flavor_Forge.Entities;
using Flavor_Forge.Services;

namespace Flavor_Forge.Operations.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Check if the "userId" cookie exists
            if (Request.Cookies.ContainsKey("userId"))
            {
                // Redirect to home
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (await _authServices.UserExists(user.Username))
            {
                TempData["ErrorMessage"] = "Username / Email already exists";
                return View(user);
            }

            var registeredUser = await _authServices.Register(user);

            if (registeredUser == null)
            {
                return StatusCode(500, "An error occurred during registration.");
            }

            var options = new CookieOptions { HttpOnly = true, Secure = true };
            Response.Cookies.Append("UserId", registeredUser.UserId.ToString(), options);
            Response.Cookies.Append("Username", registeredUser.Username, options);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if the "userId" cookie exists
            if (Request.Cookies.ContainsKey("userId"))
            {
                // Redirect to home
                return RedirectToAction("Index", "Home");
            }

            // If no cookie exists, show the login page
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid login data.";
                return View(user);
            }

            // Authenticate User
            var validUser = await _authServices.Login(user.Username, user.Password);

            // If user is invalid
            if (validUser == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return View(user);
            }

            var options = new CookieOptions { HttpOnly = true, Secure = true };
            Response.Cookies.Append("UserId", validUser.UserId.ToString(), options);
            Response.Cookies.Append("Username", validUser.Username, options);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Remove authentication cookie
            Response.Cookies.Delete("UserId");

            return RedirectToAction("Index", "Home");
        }
    }
}
