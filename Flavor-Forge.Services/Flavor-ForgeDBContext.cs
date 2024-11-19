using Microsoft.EntityFrameworkCore;
using Flavor_Forge.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services
{
    public class Flavor_ForgeDBContext : DbContext
    {
        public Flavor_ForgeDBContext(DbContextOptions<Flavor_ForgeDBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
    }
}
