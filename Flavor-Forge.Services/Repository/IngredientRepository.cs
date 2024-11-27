using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services.Repository
{
    public class IngredientRepository : IIngredientServices
    {
        private readonly Flavor_ForgeDBContext _flavor_forgeDBContext;
        public IngredientRepository(Flavor_ForgeDBContext context)
        {
            _flavor_forgeDBContext = context;
        }

        public List<Ingredient> GetIngredients(int recipeId)
        {
            return _flavor_forgeDBContext.Ingredients
                .Where(i => i.RecipeId == recipeId)
                .ToList();
        }
    }
}
