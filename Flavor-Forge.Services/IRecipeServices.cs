using Flavor_Forge.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services
{
    public interface IRecipeServices
    {
        // Get recipes by user
        List<Recipe> GetRecipesByUserId(int userId);
        Recipe GetRecipe(int recipeId);
        Recipe CreateRecipe(Recipe recipe);
        Recipe UpdateRecipe(Recipe recipe);
        Recipe DeleteRecipe(int recipeId);
    }
}
