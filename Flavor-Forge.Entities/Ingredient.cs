using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Entities
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }
        public string? Name { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }
        public virtual Recipe? Recipe { get; set; }
    }
}
