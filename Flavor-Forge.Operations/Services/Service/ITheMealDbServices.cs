using Flavor_Forge.Entities;

namespace Flavor_Forge.Operations.Services.Service
{
    public interface ITheMealDbServices
    {
        Task<(Recipe? Recipe, List<Ingredient> Ingredients)> GetRecipeDetailsAsync(string mealName);
        Task<List<dynamic>> SearchMealsByIngredientsAsync(string ingredients);
    }
}
