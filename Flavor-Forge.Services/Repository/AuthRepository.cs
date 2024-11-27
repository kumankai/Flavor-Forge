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

        public async Task<User?> RegisterAsync(User user)
        {

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

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _flavor_forgeDBContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (await CheckPassword(password, user.UserId))
            {
                return user;
            }

            // Returns user or null
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
