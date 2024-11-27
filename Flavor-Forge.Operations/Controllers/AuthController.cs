using Microsoft.AspNetCore.Mvc;
using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Flavor_Forge.Operations.Services;

namespace Flavor_Forge.Operations.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthServices _authServices;
        private readonly ICookiesServices _cookiesServices;

        public AuthController(IAuthServices authServices, ICookiesServices cookiesServices)
        {
            _authServices = authServices;
            _cookiesServices = cookiesServices;
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Check if the "userId" cookie exists
            if (_cookiesServices.GetCookie("UserId") != null)
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

            if (await _authServices.UserExistsAsync(user.Username))
            {
                TempData["ErrorMessage"] = "Username / Email already exists";
                return View(user);
            }

            var registeredUser = await _authServices.RegisterAsync(user);

            if (registeredUser == null)
            {
                return StatusCode(500, "An error occurred during registration.");
            }

            _cookiesServices.SetCookie("UserId", registeredUser.UserId.ToString());
            _cookiesServices.SetCookie("Username", registeredUser.Username.ToString());

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if the "userId" cookie exists
            if (_cookiesServices.GetCookie("UserId") != null)
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
            var validUser = await _authServices.LoginAsync(user.Username, user.Password);

            // If user is invalid
            if (validUser == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return View(user);
            }

            _cookiesServices.SetCookie("UserId", validUser.UserId.ToString());
            _cookiesServices.SetCookie("Username", validUser.Username.ToString());

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Remove authentication cookies
            _cookiesServices.DeleteCookie("UserId");
            _cookiesServices.DeleteCookie("Username");

            return RedirectToAction("Index", "Home");
        }
    }
}
