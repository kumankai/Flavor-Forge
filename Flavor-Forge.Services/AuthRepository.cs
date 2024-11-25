using Flavor_Forge.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flavor_Forge.Services
{
    public class AuthRepository : IAuthServices
    {
        private readonly Flavor_ForgeDBContext _flavor_forgeDBContext;

        public AuthRepository(Flavor_ForgeDBContext context)
        {
            _flavor_forgeDBContext = context;
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await _flavor_forgeDBContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower()
                                     && u.Password == password);

            if (user == null)
            {
                return false;
            }

            user.IsLoggedIn = true;
            return true;
        }

        public async Task<bool> Logout(int userId)
        {
            var user = await _flavor_forgeDBContext.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.IsLoggedIn = false;
            return true;
        }

        public async Task<User?> Register(User user)
        {

            if (await UserExists(user.Username))
            {
                return null;
            }

            _flavor_forgeDBContext.Users.Add(user);
            _flavor_forgeDBContext.SaveChanges();
            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            var user = await _flavor_forgeDBContext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());

            if (!user)
            {
                return false;
            }

            return true;
        }
    }
}
