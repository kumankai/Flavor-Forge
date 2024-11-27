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
    }
}
