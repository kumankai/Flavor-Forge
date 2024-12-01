using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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

        public Ingredient SaveIngredient(Ingredient ingredient)
        {
            _flavor_forgeDBContext.Ingredients.Add(ingredient);
            _flavor_forgeDBContext.SaveChanges();
            return ingredient;
        }

        public void SaveIngredients(List<Ingredient> ingredients, int recipeId)
        {
            if (ingredients == null || !ingredients.Any())
            {
                throw new ArgumentException("Ingredients list cannot be null or empty.");
            }

            foreach (var ingredient in ingredients)
            {
                if (ingredient != null && !string.IsNullOrEmpty(ingredient.IngredientName))
                {
                    ingredient.RecipeId = recipeId;
                    _flavor_forgeDBContext.Ingredients.Add(ingredient);
                }
            }

            _flavor_forgeDBContext.SaveChanges();
        }

        public async Task UpdateIngredients(List<Ingredient> ingredients, int recipeId)
        {
            if (ingredients == null || !ingredients.Any())
            {
                throw new ArgumentException("Ingredients list cannot be null or empty.");
            }

            // Fetch all existing ingredients for the recipe
            var existingIngredients = _flavor_forgeDBContext.Ingredients
                .Where(i => i.RecipeId == recipeId)
                .ToList();

            // Remove all existing ingredients for the recipe
            _flavor_forgeDBContext.Ingredients.RemoveRange(existingIngredients);

            // Add the new ingredients
            foreach (var ingredient in ingredients)
            {
                ingredient.RecipeId = recipeId; // Ensure the RecipeId is set for each ingredient
                _flavor_forgeDBContext.Ingredients.Add(ingredient);
            }

            // Save changes to the database
            await _flavor_forgeDBContext.SaveChangesAsync();
        }

    }
}
