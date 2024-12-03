/*
 * This repository defines the services for recipe actions.
 * This includes actions like searching for recipes or creating them.
 */

using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services.Repository
{
    public class RecipeRepository : IRecipeServices
    {
        private readonly Flavor_ForgeDBContext _flavor_forgeDBContext;
        public RecipeRepository(Flavor_ForgeDBContext context)
        {
            _flavor_forgeDBContext = context;
        }

        // Get all recipes saved by a user to their id
        public List<Recipe> GetRecipesByUserId(int userId)
        {
            return _flavor_forgeDBContext.Recipes
                .Where(r => r.UserId == userId)
                .ToList();
        }

        //Recipe GetRecipeById(recipeId);
        public Recipe GetRecipeById(int recipeId)
        {
            return _flavor_forgeDBContext.Recipes.FirstOrDefault(r => r.RecipeId == recipeId);
        }

        //Recipe GetRecipeByName(string recipeName, int userId);
        public Recipe GetRecipeByName(string recipeName, int userId)
        {
            return _flavor_forgeDBContext.Recipes.FirstOrDefault(r => r.UserId == userId && r.RecipeName == recipeName);
        }

        //Recipe CreateRecipe(Recipe recipe);
        public Recipe CreateRecipe(Recipe recipe)
        {
            // Save recipe
            _flavor_forgeDBContext.Recipes.Add(recipe);
            _flavor_forgeDBContext.SaveChanges();
            return recipe;
        }

        //Recipe UpdateRecipe(Recipe recipe);
        public async Task<Recipe> UpdateRecipeAsync(Recipe recipe)
        {
            _flavor_forgeDBContext.Recipes.Update(recipe);
            _flavor_forgeDBContext.SaveChanges();
            return recipe;
        }

        //Recipe DeleteRecipe(int recipeId);
        public string DeleteRecipe(int recipeId)
        {
            _flavor_forgeDBContext.Recipes.Remove(GetRecipeById(recipeId));
            _flavor_forgeDBContext.SaveChanges();
            return "Recipe Deleted";
        }

        // Confirm if recipe is saved by the user with id provided
        public bool CheckSavedRecipe(string recipeName, int userId)
        {
            // Get a list of all recipes saved by this user then check if any of the recipes in the list are the provided recipe
            var recipes = GetRecipesByUserId(userId);
            if (recipes.Any(r => r.RecipeName == recipeName))
            {
                return true;
            }
            return false;
        }
    }
}
