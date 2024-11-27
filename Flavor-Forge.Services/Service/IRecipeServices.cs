using Flavor_Forge.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services.Service
{
    public interface IRecipeServices
    {
        // Get recipes by user
        List<Recipe> GetRecipesByUserId(int userId);
        Recipe GetRecipeById(int recipeId);
        Recipe GetRecipeByName(string recipeName, int userId);
        Recipe CreateRecipe(Recipe recipe);
        Recipe UpdateRecipe(Recipe recipe);
        string DeleteRecipe(int recipeId);
        bool CheckSavedRecipe(string recipeName, int userId);
        List<string> ParseIngredients(Recipe recipe);
    }
}
