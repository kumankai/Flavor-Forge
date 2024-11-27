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

        public List<Recipe> GetRecipesByUserId(int userId)
        {
            return _flavor_forgeDBContext.Recipes
                .Where(r => r.UserId == userId)
                .ToList();
        }
        //Recipe GetRecipe(int recipeId);
        public Recipe GetRecipe(int recipeId)
        {
            return _flavor_forgeDBContext.Recipes.FirstOrDefault(r => r.RecipeId == recipeId);
        }
        //Recipe CreateRecipe(Recipe recipe);
        public Recipe CreateRecipe(Recipe recipe)
        {
            _flavor_forgeDBContext.Recipes.Add(recipe);
            _flavor_forgeDBContext.SaveChanges();
            return recipe;
        }
        //Recipe UpdateRecipe(Recipe recipe);
        public Recipe UpdateRecipe(Recipe recipe)
        {
            _flavor_forgeDBContext.Recipes.Update(recipe);
            _flavor_forgeDBContext.SaveChanges();
            return recipe;
        }
        //Recipe DeleteRecipe(int recipeId);
        public string DeleteRecipe(int recipeId)
        {
            _flavor_forgeDBContext.Recipes.Remove(GetRecipe(recipeId));
            _flavor_forgeDBContext.SaveChanges();
            return "Recipe Deleted";
        }
        public bool CheckSavedRecipe(string recipeName, int userId)
        {
            var recipes = GetRecipesByUserId(userId);
            if (recipes.Any(r => r.RecipeName == recipeName))
            {
                return true;
            }
            return false;
        }
    }
}
