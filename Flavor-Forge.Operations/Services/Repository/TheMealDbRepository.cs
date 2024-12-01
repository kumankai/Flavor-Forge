/*
 * This repository defines the services for TheMealDb API
 */

using Flavor_Forge.Entities;
using Flavor_Forge.Operations.Services.Service;
using System.Text.Json;

namespace Flavor_Forge.Operations.Services.Repository
{
    public class TheMealDbRepository : ITheMealDbServices
    {
        // URL of Web API
        private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1/";

        private readonly HttpClient _httpClient;

        public TheMealDbRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Parses quantity from a string of measurement
        // e.g. string: "1 cup water"
        private decimal? ParseQuantity(string measure)
        {
            // Splits the string by spaces
            var parts = measure.Split(' ');

            // If quantity exists
            if (decimal.TryParse(parts[0], out var quantity))
            {
                return quantity;
            }
            return null;
        }

        // Parses measurement unit from a string of measurement
        // e.g. string: "1 cup water"
        private string? ParseUnit(string measure)
        {
            // Split the string by spaces
            var parts = measure.Split(' ');
            // If unit exists return, else null
            return parts.Length > 1 ? parts[1] : null;
        }

        // Calls the API to retrieve a recipe by recipe name
        public async Task<(Recipe? Recipe, List<Ingredient> Ingredients)> GetRecipeDetailsAsync(string mealName)
        {
            string url = $"{BaseUrl}search.php?s={mealName.Trim()}";

            // Call the API
            string jsonResponse = await _httpClient.GetStringAsync(url);
            // Parse the JSON response
            var response = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(jsonResponse);

            // If response contains main key "meals" and is not empty
            if (response != null && response.ContainsKey("meals") && response["meals"] != null)
            {
                var mealData = response["meals"][0];

                // Create a new Recipe object from response
                var recipe = new Recipe
                {
                    RecipeName = mealData["strMeal"],
                    Instructions = mealData["strInstructions"],
                    ImageUrl = mealData["strMealThumb"],
                    Author = "TheMealDB"
                };

                // Create a list of ingredient objects
                var ingredients = new List<Ingredient>();

                // Save all ingredients to database
                // API response only allows a maximum of 20 ingredients
                for (int i = 1; i <= 20; i++)
                {
                    // Ingredient name key
                    string ingredientKey = $"strIngredient{i}";
                    // Measurement key
                    string measureKey = $"strMeasure{i}";

                    if (mealData.ContainsKey(ingredientKey) && mealData.ContainsKey(measureKey))
                    {
                        // Grab ingredient name
                        string ingredientName = mealData[ingredientKey];
                        // Grab measurement
                        string measure = mealData[measureKey];

                        // Ignore empty 
                        if (!string.IsNullOrWhiteSpace(ingredientName))
                        {
                            // Save ingredient to DB
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

        // Call API to search for recipes by list of ingredients
        public async Task<List<dynamic>> SearchMealsByIngredientsAsync(string ingredients)
        {
            // List of meal object
            var meals = new List<dynamic>();
            // Split list of ingredients by commas
            IEnumerable<string> ingredientList = ingredients.Split(',').Select(i => i.Trim());

            // For every ingredient, call API
            foreach (var ingredient in ingredientList)
            {
                // URL
                string url = $"{BaseUrl}filter.php?i={ingredient.Trim()}";
                // Call API
                string jsonResponse = await _httpClient.GetStringAsync(url);
                // Parse JSON response
                var response = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(jsonResponse);

                if (response != null && response.ContainsKey("meals") && response["meals"] != null)
                {
                    // Save all recipes with name and imageURL
                    var mealsForIngredient = response["meals"].Select(meal => new
                    {
                        Name = meal["strMeal"],
                        ImageUrl = meal["strMealThumb"]
                    });

                    // Add all saved recipes to list of meal objects
                    meals.AddRange(mealsForIngredient);
                }
            }

            // Return unique recipes
            return meals.DistinctBy(m => m.Name).ToList();
        }
    }
}
