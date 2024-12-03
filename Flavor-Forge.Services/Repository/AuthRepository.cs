using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Microsoft.EntityFrameworkCore;

namespace Flavor_Forge.Services.Repository
{
    public class AuthRepository : IAuthServices
    {
        // Database context for accessing the Flavor Forge database.
        private readonly Flavor_ForgeDBContext _flavor_forgeDBContext;

        // Constructor to initialize the repository with the database context.
        public AuthRepository(Flavor_ForgeDBContext context)
        {
            _flavor_forgeDBContext = context;
        }


        /// <summary>
        /// Registers a new user if the username does not already exist.
        /// </summary>
        /// <param name="user">The user object containing registration details.</param>
        /// <returns>The registered user object if successful; otherwise, null.</returns>
        public async Task<User?> RegisterAsync(User user)
        {
            // Check if the username already exists in the database.
            if (await UserExistsAsync(user.Username))
            {
                return null;
            }
            
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password));
            user.Password = Convert.ToBase64String(bytes);
            
            _flavor_forgeDBContext.Users.Add(user);
            _flavor_forgeDBContext.SaveChanges();
            return user;
        }

        /// <summary>
        /// Authenticates a user by verifying their username and password.
        /// </summary>
        /// <param name="username">The username provided for login.</param>
        /// <param name="password">The password provided for login.</param>
        /// <returns>The user object if authentication is successful; otherwise, null.</returns>
        public async Task<User?> LoginAsync(string username, string password)
        {
            // Find the user in the database by username (case-insensitive).
            var user = await _flavor_forgeDBContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                return null;
            }

            if (await CheckPassword(password, user.UserId))
            {
                return user;
            }

            return null;
        }

        /// <summary>
        /// Simulates logging out a user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to log out.</param>
        /// <returns>True if logout is simulated successfully; otherwise, false.</returns>
        public async Task<bool> LogoutAsync(int userId)
        {

            // Attempt to find the user by their ID.
            var user = await _flavor_forgeDBContext.Users.FindAsync(userId);

            if (user == null)
            {
                // Logout not successful
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a user with the specified username exists.
        /// </summary>
        /// <param name="username">The username to check for existence.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        public async Task<bool> UserExistsAsync(string username)
        {
            // Check if any user in the database has the specified username (case-insensitive).
            var user = await _flavor_forgeDBContext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());

            if (!user)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies if the given password matches the stored password for the specified user.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="userId">The ID of the user whose password is being verified.</param>
        /// <returns>True if the password matches; otherwise, false.</returns>
        public async Task<bool> CheckPassword(string password, int userId) 
        {
            // Hash the provided password using SHA256.
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            var hashed_password = Convert.ToBase64String(bytes);
          
            // Rettrieve user pass from db 
            var user = await _flavor_forgeDBContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            //Compare the stored hashed password with the newly hashed password.
            if (user.Password == hashed_password)
            {
                return true;
            }
            return false;
        }
    }
}
