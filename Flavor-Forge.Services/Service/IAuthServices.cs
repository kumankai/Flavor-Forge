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
        Task<User?> Register(User user);
        Task<User?> Login(string username, string password);
        Task<bool> Logout(int userId);
        Task<bool> UserExists(string username);
    }
}
