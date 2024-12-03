/*
 * This service is for handling User related actions like creating or deleting them
 * Used for Dependecy Injection
 */

using Flavor_Forge.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services.Service
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
