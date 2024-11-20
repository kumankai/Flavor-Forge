using Flavor_Forge.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services
{
    public interface IUserServices
    {
        List<User> GetUsers();
        User GetUser(int userId);
        User CreateUser(User user);
        User UpdateUser(User user);
        string DeleteUser(int userId);
    }
}
