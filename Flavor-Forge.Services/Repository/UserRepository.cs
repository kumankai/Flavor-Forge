using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services.Repository
{
    public class UserRepository : IUserServices
    {
        private readonly Flavor_ForgeDBContext _flavor_forgeDBContext;
        public UserRepository(Flavor_ForgeDBContext context)
        {
            _flavor_forgeDBContext = context;
        }

        public List<User> GetUsers()
        {
            return _flavor_forgeDBContext.Users.ToList();
        }

        public User GetUser(int userId)
        {
            return _flavor_forgeDBContext.Users.FirstOrDefault(e => e.UserId == userId);
        }

        public User CreateUser(User user)
        {
            _flavor_forgeDBContext.Users.Add(user);
            _flavor_forgeDBContext.SaveChanges();
            return user;
        }

        public User UpdateUser(User user)
        {
            _flavor_forgeDBContext.Users.Update(user);
            _flavor_forgeDBContext.SaveChanges();
            return user;
        }

        public string DeleteUser(int userId)
        {
            _flavor_forgeDBContext.Users.Remove(GetUser(userId));
            _flavor_forgeDBContext.SaveChanges();
            return "User deleted";
        }
    }
}
