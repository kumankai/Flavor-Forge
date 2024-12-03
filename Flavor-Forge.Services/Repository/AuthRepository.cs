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
        // Database context for accessing the Flavor Forge database.
        private readonly Flavor_ForgeDBContext _flavor_forgeDBContext;

        // Constructor to initialize the repository with the database context.
        public AuthRepository(Flavor_ForgeDBContext context)
        {
            _flavor_forgeDBContext = context;
        }

        public async Task<User?> RegisterAsync(User user)
        {

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

        public async Task<User?> LoginAsync(string username, string password)
        {
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

        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _flavor_forgeDBContext.Users.FindAsync(userId);

            if (user == null)
            {
                // Logout not successful
                return false;
            }

            // Logout successful
            return true;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await _flavor_forgeDBContext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());

            if (!user)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> CheckPassword(string password, int userId) 
        {
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
