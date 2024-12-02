using Microsoft.AspNetCore.Mvc;
using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Flavor_Forge.Operations.Services.Service;

namespace Flavor_Forge.Operations.Controllers
{
    public class AuthController : Controller
    {
        // Service dependencies for authentication and cookie management
        private readonly IAuthServices _authServices;
        private readonly ICookiesServices _cookiesServices;

        // Constructor initializes the services
        public AuthController(IAuthServices authServices, ICookiesServices cookiesServices)
        {
            _authServices = authServices;
            _cookiesServices = cookiesServices;
        }

        /// <summary>
        /// Handles GET requests to the registration page.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            // Check if the "userId" cookie exists, indicating the user is already logged in
            if (_cookiesServices.GetCookie("UserId") != null)
            {
                // Redirect authenticated users to the to home page
                return RedirectToAction("Index", "Home");
            }
            // Return the registration view for new users
            return View();
        }

        /// <summary>
        /// Handles POST requests for user registration.
        /// </summary>
        /// <param name="user">The user details submitted for registration.</param>
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Return the form with validation errors
                return View(user);
            }

            // Check if the username/email already exists
            if (await _authServices.UserExistsAsync(user.Username))
            {
                TempData["ErrorMessage"] = "Username / Email already exists";
                return View(user);
            }


            // Attempt to register the user
            var registeredUser = await _authServices.RegisterAsync(user);


            // Handle errors during registration
            if (registeredUser == null)
            {
                return StatusCode(500, "An error occurred during registration.");
            }

            // Set authentication cookies for the registered user
            _cookiesServices.SetCookie("UserId", registeredUser.UserId.ToString());
            _cookiesServices.SetCookie("Username", registeredUser.Username.ToString());

            // Redirect to the home page upon successful registration
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Handles GET requests to the login page.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            // Check if the "UserId" cookie exists, indicating the user is already logged in
            if (_cookiesServices.GetCookie("UserId") != null)
            {
                // Redirect authenticated users to the home page
                return RedirectToAction("Index", "Home");
            }

            // If no cookie exists, show the login page
            // Return the login view for new users
            return View();
        }


        /// <summary>
        /// Handles POST requests for user login.
        /// </summary>
        /// <param name="user">The user details submitted for login.</param>
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            // Check if the user object is valid
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid login data.";
                return View(user);
            }

            // Authenticate User
            var validUser = await _authServices.LoginAsync(user.Username, user.Password);

            // Handle invalid login attempts
            if (validUser == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return View(user);
            }

            // Set authentication cookies for the logged-in user
            _cookiesServices.SetCookie("UserId", validUser.UserId.ToString());
            _cookiesServices.SetCookie("Username", validUser.Username.ToString());

            // Redirect to the home page upon successful login
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Handles POST requests to log out the user.
        /// </summary>
        [HttpPost]
        public IActionResult Logout()
        {
            // Remove authentication cookies
            _cookiesServices.DeleteCookie("UserId");
            _cookiesServices.DeleteCookie("Username");

            // Redirect to the home page after logout
            return RedirectToAction("Index", "Home");
        }
    }
}
