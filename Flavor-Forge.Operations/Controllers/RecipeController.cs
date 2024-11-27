using Flavor_Forge.Entities;
using Flavor_Forge.Services.Service;
using Flavor_Forge.Operations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using System.Net.Http;
using System.Text.Json;

namespace Flavor_Forge.Operations.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeServices _recipeServices;
        private readonly ICookiesServices _cookiesServices;
        private readonly IImageServices _imageServices;

        public RecipeController(IRecipeServices recipeServices, ICookiesServices cookiesServices, IImageServices imageServices)
        {
            _recipeServices = recipeServices;
            _cookiesServices = cookiesServices;
            _imageServices = imageServices;
        }

        [HttpGet]
        public IActionResult Add()
        {
            // Check if the "userId" cookie doesnt exists
            if (_cookiesServices.GetCookie("UserId") == null)
            {
                // Redirect to Login
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Recipe recipe, IFormFile imageFile)
        {
            try
            {
                string userIdCookie = _cookiesServices.GetCookie("UserId");

                if (userIdCookie == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(userIdCookie);
                string username = _cookiesServices.GetCookie("Username");

                if (imageFile != null)
                {
                    if (!_imageServices.ValidateImage(imageFile, out string errorMessage))
                    {
                        TempData["ErrorMessage"] = errorMessage;
                        return RedirectToAction("Add");
                    }

                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recipe-images");
                    recipe.ImageUrl = await _imageServices.SaveImageAsync(imageFile, folderPath);
                }

                recipe.UserId = userId;
                recipe.Author = username;

                if (recipe.Ingredients == null)
                {
                    recipe.Ingredients = new List<string>();
                }

                _recipeServices.CreateRecipe(recipe);

                TempData["SuccessMessage"] = "Recipe added successfully!";
                return RedirectToAction("Profile", "User");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding recipe: " + ex.Message;
                return RedirectToAction("Add");
            }
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
        [HttpPost]
        public IActionResult Details(Recipe recipe)
        {
            try
            {
                string userIdCookie = _cookiesServices.GetCookie("UserId");

                // Check if user is logged in
                if (userIdCookie == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(userIdCookie);

                // Check if user has already saved this recipe
                if (_recipeServices.CheckSavedRecipe(recipe.RecipeName, userId))
                {
                    TempData["ErrorMessage"] = "You have already saved this recipe!";
                    return RedirectToAction("Details", new { mealName = recipe.RecipeName });
                }

                // Set the UserId for the recipe
                recipe.UserId = userId;

                // Save the recipe
                _recipeServices.CreateRecipe(recipe);

                TempData["SuccessMessage"] = "Recipe Saved Successfully!";
                return RedirectToAction("Details", new { mealName = recipe.RecipeName });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error saving recipe.";
                return RedirectToAction("Details", new { mealName = recipe.RecipeName });
            }
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

        [HttpPost]
        public IActionResult DeleteRecipe(int recipeId)
        {
            try
            {
                string userIdCookie = _cookiesServices.GetCookie("UserId");

                if (userIdCookie == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(userIdCookie);
                string username = _cookiesServices.GetCookie("Username");

                var recipe = _recipeServices.GetRecipe(recipeId);

                if (recipe == null || recipe.UserId != userId)
                {
                    TempData["ErrorMessage"] = "Recipe not found or you don't have permission to delete it.";
                    return RedirectToAction("Profile", "User");
                }

                if (recipe.Author == username && !string.IsNullOrEmpty(recipe.ImageUrl))
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recipe-images");
                    _imageServices.DeleteImage(recipe.ImageUrl, folderPath);
                }

                _recipeServices.DeleteRecipe(recipeId);

                TempData["SuccessMessage"] = "Recipe deleted successfully.";
                return RedirectToAction("Profile", "User");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting recipe: " + ex.Message;
                return RedirectToAction("Profile", "User");
            }
        }
    }
}
