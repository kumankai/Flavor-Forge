using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void UpdateIngredients(List<Ingredient> ingredients, int recipeId)
        {
            if (ingredients == null || !ingredients.Any())
            {
                throw new ArgumentException("Ingredients list cannot be null or empty.");
            }

            // Fetch existing ingredients for the recipe
            var existingIngredients = _flavor_forgeDBContext.Ingredients
                .Where(i => i.RecipeId == recipeId)
                .ToList();

            // Remove ingredients that are no longer in the updated list
            foreach (var existingIngredient in existingIngredients)
            {
                if (!ingredients.Any(i => i.IngredientId == existingIngredient.IngredientId))
                {
                    _flavor_forgeDBContext.Ingredients.Remove(existingIngredient);
                }
            }

            // Update or add ingredients
            foreach (var ingredient in ingredients)
            {
                var existingIngredient = existingIngredients
                    .FirstOrDefault(i => i.IngredientId == ingredient.IngredientId);

                if (existingIngredient != null)
                {
                    // Update existing ingredient
                    existingIngredient.IngredientName = ingredient.IngredientName;
                    existingIngredient.Quantity = ingredient.Quantity;
                    existingIngredient.Unit = ingredient.Unit;
                }
                else
                {
                    // Add new ingredient
                    ingredient.RecipeId = recipeId;
                    _flavor_forgeDBContext.Ingredients.Add(ingredient);
                }
            }

            // Save changes to the database
            _flavor_forgeDBContext.SaveChanges();
        }
    }
}
