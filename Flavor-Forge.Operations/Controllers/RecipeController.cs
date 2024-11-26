using Flavor_Forge.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using System.Net.Http;
using System.Text.Json;

namespace Flavor_Forge.Operations.Controllers
{
    public class RecipeController : Controller
    {

        public IActionResult Add()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(string mealName)
        {
            using var httpClient = new HttpClient();
            string url = $"https://www.themealdb.com/api/json/v1/1/search.php?s={mealName.Trim()}";

            try
            {
                string jsonResponse = await httpClient.GetStringAsync(url);
                var response = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(jsonResponse);

                if (response != null && response.ContainsKey("meals") && response["meals"] != null)
                {
                    var mealData = response["meals"][0];

                    var recipe = new Recipe
                    {
                        RecipeName = mealData["strMeal"],
                        Instructions = mealData["strInstructions"],
                        ImageUrl = mealData["strMealThumb"],
                        Ingredients = new List<string>(),
                        Author = "TheMealDB"
                    };

                    // Process ingredients
                    for (int i = 1; i <= 20; i++)
                    {
                        string ingredientKey = $"strIngredient{i}";
                        string measureKey = $"strMeasure{i}";

                        if (mealData.ContainsKey(ingredientKey) && mealData.ContainsKey(measureKey))
                        {
                            string ingredientName = mealData[ingredientKey];
                            string measure = mealData[measureKey];

                            if (!string.IsNullOrWhiteSpace(ingredientName) && !string.IsNullOrWhiteSpace(measure))
                            {
                                // Combine measure and ingredient name into one string
                                string fullIngredient = $"{measure} {ingredientName}".Trim();
                                recipe.Ingredients.Add(fullIngredient);
                            }
                        }
                    }

                    return View(recipe);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction("Error");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Search(string ingredients)
        {
            var meals = new List<dynamic>();
            using var httpClient = new HttpClient();

            var ingredientList = ingredients.Split(',').ToList();

            foreach (var ingredient in ingredientList)
            {
                string url = $"https://www.themealdb.com/api/json/v1/1/filter.php?i={ingredient.Trim()}";

                try
                {
                    string jsonResponse = await httpClient.GetStringAsync(url);
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching meals for ingredient '{ingredient}': {ex.Message}");
                }
            }

            ViewBag.Meals = meals.DistinctBy(m => m.Name).ToList();
            return View();
        }
    }
}
