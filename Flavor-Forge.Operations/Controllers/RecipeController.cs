﻿using Flavor_Forge.Entities;
using Flavor_Forge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using System.Net.Http;
using System.Text.Json;

public class RecipeController : Controller
{
    private readonly IRecipeServices _recipeServices;

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
                        Ingredients = new List<string>(),
                        Author = "TheMealDB" // or you could leave it null
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
            var mealNames = new List<string>();
            using var httpClient = new HttpClient();

            // Split the comma-separated string into a list
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
                        var mealsForIngredient = response["meals"]
                            .Select(meal => meal["strMeal"])
                            .ToList();

                        mealNames.AddRange(mealsForIngredient);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching meals for ingredient '{ingredient}': {ex.Message}");
                }
            }

            // Remove duplicates and store in ViewBag
            ViewBag.MealNames = mealNames.Distinct().ToList();
            return View();
        }
    }
}