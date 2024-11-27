using Flavor_Forge.Entities;
using Flavor_Forge.Operations.Services.Service;
using System.Text.Json;

namespace Flavor_Forge.Operations.Services.Repository
{
    public class TheMealDbRepository : ITheMealDbServices
    {
        private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1/";

        private readonly HttpClient _httpClient;

        public TheMealDbRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private decimal? ParseQuantity(string measure)
        {
            // Implement logic to parse the quantity from the measure string
            var parts = measure.Split(' ');
            if (decimal.TryParse(parts[0], out var quantity))
            {
                return quantity;
            }
            return null;
        }

        private string? ParseUnit(string measure)
        {
            // Implement logic to parse the unit from the measure string
            var parts = measure.Split(' ');
            return parts.Length > 1 ? parts[1] : null;
        }

        public async Task<(Recipe? Recipe, List<Ingredient> Ingredients)> GetRecipeDetailsAsync(string mealName)
        {
            string url = $"{BaseUrl}search.php?s={mealName.Trim()}";

            string jsonResponse = await _httpClient.GetStringAsync(url);
            var response = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(jsonResponse);

            if (response != null && response.ContainsKey("meals") && response["meals"] != null)
            {
                var mealData = response["meals"][0];

                var recipe = new Recipe
                {
                    RecipeName = mealData["strMeal"],
                    Instructions = mealData["strInstructions"],
                    ImageUrl = mealData["strMealThumb"],
                    Author = "TheMealDB"
                };

                var ingredients = new List<Ingredient>();

                for (int i = 1; i <= 20; i++)
                {
                    string ingredientKey = $"strIngredient{i}";
                    string measureKey = $"strMeasure{i}";

                    if (mealData.ContainsKey(ingredientKey) && mealData.ContainsKey(measureKey))
                    {
                        string ingredientName = mealData[ingredientKey];
                        string measure = mealData[measureKey];

                        if (!string.IsNullOrWhiteSpace(ingredientName))
                        {
                            ingredients.Add(new Ingredient
                            {
                                IngredientName = ingredientName,
                                Quantity = ParseQuantity(measure),
                                Unit = ParseUnit(measure)
                            });
                        }
                    }
                }

                return (recipe, ingredients);
            }
            return (null, new List<Ingredient>());
        }

        public async Task<List<dynamic>> SearchMealsByIngredientsAsync(string ingredients)
        {
            var meals = new List<dynamic>();
            IEnumerable<string> ingredientList = ingredients.Split(',').Select(i => i.Trim());

            foreach (var ingredient in ingredientList)
            {
                string url = $"{BaseUrl}filter.php?i={ingredient.Trim()}";
                string jsonResponse = await _httpClient.GetStringAsync(url);
                var response = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(jsonResponse);

                if (response != null && response.ContainsKey("meals") && response["meals"] != null)
                {
                    var mealsForIngredient = response["meals"].Select(meal => new
                    {
                        Name = meal["strMeal"],
                        ImageUrl = meal["strMealThumb"]
                    });

                    meals.AddRange(mealsForIngredient);
                }
            }

            return meals.DistinctBy(m => m.Name).ToList();
        }
    }
}
