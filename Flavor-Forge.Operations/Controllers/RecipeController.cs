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
        private readonly ITheMealDbServices _theMealDbServices;
        private readonly IIngredientServices _ingredientServices;

        public RecipeController(IRecipeServices recipeServices, ICookiesServices cookiesServices, IImageServices imageServices, ITheMealDbServices theMealDbServices, IIngredientServices ingredientServices)
        {
            _recipeServices = recipeServices;
            _cookiesServices = cookiesServices;
            _imageServices = imageServices;
            _theMealDbServices = theMealDbServices;
            _ingredientServices = ingredientServices;
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
        public async Task<IActionResult> Add(Recipe recipe, IFormFile imageFile, List<Ingredient> Ingredients)
        {
            try
            {
                // Authenticate
                string? userIdCookie = _cookiesServices.GetCookie("UserId");
                if (userIdCookie == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(userIdCookie);
                string? username = _cookiesServices.GetCookie("Username");

                // Handle image upload
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

                // Set recipe properties
                recipe.UserId = userId;
                recipe.Author = username;

                // Save the recipe first to get the RecipeId
                var savedRecipe = _recipeServices.CreateRecipe(recipe);

                // Save each ingredient
                if (Ingredients != null && Ingredients.Any())
                {
                    foreach (var ingredient in Ingredients)
                    {
                        if (ingredient != null && !string.IsNullOrEmpty(ingredient.IngredientName))
                        {
                            ingredient.RecipeId = savedRecipe.RecipeId;
                            _ingredientServices.SaveIngredient(ingredient);
                        }
                    }
                }

                TempData["SuccessMessage"] = "Recipe added successfully!";
                return RedirectToAction("Profile", "User");
            }
            catch (Exception ex)
            {
                // Add more detailed error logging
                Console.WriteLine($"Error details: {ex}");
                TempData["ErrorMessage"] = "Error adding recipe: " + ex.Message;
                return RedirectToAction("Add");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string mealName, string mealAuthor)
        {
            try
            {
                string? currentUsername = _cookiesServices.GetCookie("Username");
                string? userIdCookie = _cookiesServices.GetCookie("UserId");

                if (userIdCookie == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(userIdCookie);

                // If the recipe author is the current user, get it from our database
                if (currentUsername == mealAuthor)
                {
                    // Get the recipe from database by name and userId
                    var recipe = _recipeServices.GetRecipe(userId);
                    // Get ingredients for this recipe
                    var ingredients = _ingredientServices.GetIngredients(recipe.RecipeId);
                    ViewBag.Ingredients = ingredients;
                    return View(recipe);
                    
                }
                // If the recipe author is not the current user, get it from TheMealDB
                else
                {
                    var (recipe, ingredients) = await _theMealDbServices.GetRecipeDetailsAsync(mealName);
                    ViewBag.Ingredients = ingredients;
                    return View(recipe);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error retrieving recipe: " + ex.Message;
                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        public IActionResult Details(Recipe recipe, List<Ingredient> ingredients)
        {
            try
            {
                string? userIdCookie = _cookiesServices.GetCookie("UserId");

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

                // Save the recipe and get the saved recipe with its ID
                var savedRecipe = _recipeServices.CreateRecipe(recipe);

                // Save ingredients if they exist
                if (ingredients != null && ingredients.Any())
                {
                    foreach (var ingredient in ingredients)
                    {
                        if (ingredient != null && !string.IsNullOrEmpty(ingredient.IngredientName))
                        {
                            // Set the RecipeId for each ingredient
                            ingredient.RecipeId = savedRecipe.RecipeId;
                            _ingredientServices.SaveIngredient(ingredient);
                        }
                    }
                }

                TempData["SuccessMessage"] = "Recipe Saved Successfully!";
                return RedirectToAction("Details", new { mealName = recipe.RecipeName });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving recipe: " + ex.Message;
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
            var meals = await _theMealDbServices.SearchMealsByIngredientsAsync(ingredients);
            ViewBag.Meals = meals;
            return View();
        }

        [HttpPost]
        public IActionResult DeleteRecipe(int recipeId)
        {
            try
            {
                string? userIdCookie = _cookiesServices.GetCookie("UserId");

                if (userIdCookie == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                int userId = int.Parse(userIdCookie);
                string? username = _cookiesServices.GetCookie("Username");

                var recipe = _recipeServices.GetRecipe(recipeId);

                // If recipe author is user, delete image from wwwroot
                if (recipe.Author == username && !string.IsNullOrEmpty(recipe.ImageUrl))
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recipe-images");
                    _imageServices.DeleteImage(recipe.ImageUrl, folderPath);
                }

                _recipeServices.DeleteRecipe(recipeId);
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
