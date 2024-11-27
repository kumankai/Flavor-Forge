using Flavor_Forge.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services.Service
{
    public interface IAuthServices
    {
        Task<User?> RegisterAsync(User user);
        Task<User?> LoginAsync(string username, string password);
        Task<bool> LogoutAsync(int userId);
        Task<bool> UserExistsAsync(string username);
    }
}
