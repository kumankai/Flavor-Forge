using Flavor_Forge.Entities;
using Flavor_Forge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using System.Net.Http;
using System.Text.Json;

namespace Flavor_Forge.Operations.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeServices _recipeServices;

        public RecipeController(IRecipeServices recipeServices)
        {
            _recipeServices = recipeServices;
        }

        [HttpGet]
        public IActionResult Add()
        {
            // Check if the "userId" cookie doesnt exists
            if (!Request.Cookies.ContainsKey("userId"))
            {
                // Redirect to Login
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Recipe recipe, IFormFile ImageFile)
        {
            try
            {
                // Check if user is logged in
                if (!Request.Cookies.ContainsKey("UserId"))
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(Request.Cookies["UserId"]);
                string username = Request.Cookies["Username"]; // Make sure you have this cookie set during login

                // Validate image
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Check file size (5MB max)
                    if (ImageFile.Length > 5 * 1024 * 1024)
                    {
                        TempData["ErrorMessage"] = "Image file size must be less than 5MB";
                        return RedirectToAction("Add");
                    }

                    // Check file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                    if (!allowedTypes.Contains(ImageFile.ContentType.ToLower()))
                    {
                        TempData["ErrorMessage"] = "Only JPG and PNG images are allowed";
                        return RedirectToAction("Add");
                    }

                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recipe-images", fileName);
                    var fileUrl = $"/recipe-images/{fileName}";

                    // Create directory if it doesn't exist
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recipe-images"));

                    // Save image
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Set image URL
                    recipe.ImageUrl = fileUrl;
                }

                // Set recipe properties
                recipe.UserId = userId;
                recipe.Author = username;

                // Process ingredients if they're in the form data
                // Note: You'll need to modify this based on how you're handling ingredients in the form
                if (recipe.Ingredients == null)
                {
                    recipe.Ingredients = new List<string>();
                }

                // Save recipe
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
                // Check if user is logged in
                if (!Request.Cookies.ContainsKey("UserId"))
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(Request.Cookies["UserId"]);

                // Check if user has already saved this recipe
                var existingRecipes = _recipeServices.GetRecipesByUserId(userId);
                if (existingRecipes.Any(r => r.RecipeName == recipe.RecipeName))
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
                // Check if user is logged in
                if (!Request.Cookies.ContainsKey("UserId"))
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(Request.Cookies["UserId"]);
                string username = Request.Cookies["Username"];

                // Get the recipe
                var recipe = _recipeServices.GetRecipe(recipeId);

                // Check if recipe exists and belongs to the current user
                if (recipe == null || recipe.UserId != userId)
                {
                    TempData["ErrorMessage"] = "Recipe not found or you don't have permission to delete it.";
                    return RedirectToAction("Profile", "User");
                }

                // If the user is the author (not TheMealDB), delete the image file
                if (recipe.Author == username && !string.IsNullOrEmpty(recipe.ImageUrl))
                {
                    try
                    {
                        // Get the file path from the ImageUrl
                        string fileName = Path.GetFileName(recipe.ImageUrl);
                        string filePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "recipe-images",
                            fileName
                        );

                        // Check if file exists before attempting to delete
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with recipe deletion
                        Console.WriteLine($"Error deleting image file: {ex.Message}");
                    }
                }

                // Delete the recipe
                _recipeServices.DeleteRecipe(recipeId);

                TempData["SuccessMessage"] = "Recipe unsaved successfully.";
                return RedirectToAction("Profile", "User");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error unsaving recipe: " + ex.Message;
                return RedirectToAction("Profile", "User");
            }
        }
    }
}
