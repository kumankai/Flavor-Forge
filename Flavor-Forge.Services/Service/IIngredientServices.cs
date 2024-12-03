/*
 * This service is for handling Ingredient related actions like saving them
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
    public interface IIngredientServices
    {
        List<Ingredient> GetIngredients(int recipeId);
        Ingredient SaveIngredient(Ingredient ingredient);
        void SaveIngredients(List<Ingredient> ingredients, int recipeId);
        Task UpdateIngredients(List<Ingredient> ingredients, int recipeId);
    }
}
