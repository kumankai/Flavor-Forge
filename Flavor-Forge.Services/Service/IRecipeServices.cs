/*
 * This service is for handling Recipe related actions like searching for them or updating
 * Used for dependency injection
 */

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
        List<Recipe> GetRecipesByUserId(int userId);
        Recipe GetRecipeById(int recipeId);
        Recipe GetRecipeByName(string recipeName, int userId);
        Recipe CreateRecipe(Recipe recipe);
        Task<Recipe> UpdateRecipeAsync(Recipe recipe);
        string DeleteRecipe(int recipeId);
        bool CheckSavedRecipe(string recipeName, int userId);
    }
}
