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

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (user == null)
                return BadRequest("Invalid user data.");

            if (await _authServices.UserExists(user.Username))
                return Conflict("Username already exists.");

            var registeredUser = await _authServices.Register(user);
            if (registeredUser == null)
                return StatusCode(500, "An error occurred during registration.");

            return Ok(registeredUser);
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (user == null)
                return BadRequest("Invalid login data.");
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var loginSuccess = await _authServices.Login(user.Username, user.Password);

            if (!loginSuccess)
                return Unauthorized("Invalid username or password.");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("logout/{userId}")]
        public async Task<IActionResult> Logout(int userId)
        {
            var logoutSuccess = await _authServices.Logout(userId);
            if (!logoutSuccess)
                return BadRequest("Logout failed.");

            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Logout()
        {
            return View();
        }
    }
}
