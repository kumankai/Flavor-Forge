using Flavor_Forge.Entities;
using Flavor_Forge.Services;
using Microsoft.AspNetCore.Mvc;

public class RecipeController : Controller
{
    private readonly IRecipeServices _recipeServices;

    public RecipeController(IRecipeServices recipeServices)
    {
        _recipeServices = recipeServices;
    }

    public IActionResult Add()
    {
        return View();
    }

    public IActionResult Details(int id)
    {
        var recipe = _recipeServices.GetRecipe(id);
        if (recipe == null)
        {
            return NotFound();
        }
        return View(recipe);
    }

    public IActionResult Search()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateRecipe(Recipe recipe)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // You might want to set the UserId here based on the logged-in user
                recipe.UserId = 1; // Temporary, replace with actual user ID
                var createdRecipe = _recipeServices.CreateRecipe(recipe);
                return RedirectToAction(nameof(Details), new { id = createdRecipe.RecipeId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating recipe: " + ex.Message);
            }
        }
        return View("Add", recipe);
    }
}