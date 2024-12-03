/*
 * This repository defines the services for user handling.
 * This includes creating or deleting accounts
 */

using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Microsoft.EntityFrameworkCore;

namespace Flavor_Forge.Services.Repository
{
    public class AuthRepository : IAuthServices
    {
        private readonly Flavor_ForgeDBContext _flavor_forgeDBContext;

        public AuthRepository(Flavor_ForgeDBContext context)
        {
            _flavor_forgeDBContext = context;
        }

        // Creates and Adds account to Repository
        public async Task<User?> RegisterAsync(User user)
        {
            // Check if username given is already in use or not
            if (await UserExistsAsync(user.Username))
            {
                return null;
            }
            
            // Hashing the password
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password));
            user.Password = Convert.ToBase64String(bytes);
            
            // Adding user to database
            _flavor_forgeDBContext.Users.Add(user);
            _flavor_forgeDBContext.SaveChanges();
            return user;
        }

        // Checks if login details are correct
        public async Task<User?> LoginAsync(string username, string password)
        {
            // Find user in database whose name matches username provided
            var user = await _flavor_forgeDBContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            // If user is not found return null...
            if (user == null)
            {
                return null;
            }

            // Or if user us found compare passwords
            if (await CheckPassword(password, user.UserId))
            {
                return user;
            }

            return null;
        }

        // Log out user
        public async Task<bool> LogoutAsync(int userId)
        {
            // Check if this user is in the Database
            var user = await _flavor_forgeDBContext.Users.FindAsync(userId);

            if (user == null)
            {
                // Logout not successful
                return false;
            }

            // Logout successful
            return true;
        }

        // Ensure usernames are unique
        public async Task<bool> UserExistsAsync(string username)
        {
            // Check whether username in use or not
            var user = await _flavor_forgeDBContext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());

            if (!user)
            {
                return false;
            }

            return true;
        }

        // Compared password given provided with the one in database
        public async Task<bool> CheckPassword(string password, int userId) 
        {
            // Hash password
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            var hashed_password = Convert.ToBase64String(bytes);
          
            // Rettrieve user pass from db 
            var user = await _flavor_forgeDBContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
            
            // Compare
            if (user.Password == hashed_password)
            {
                return true;
            }
            return false;
        }
    }
}
