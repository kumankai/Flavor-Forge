using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flavor_Forge.Entities
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        public string? RecipeName { get; set; }
        public string? Instructions { get; set; }

        [Required]
        public List<string>? Ingredients { get; set; }
        public string? Author { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } 
        public virtual User? User { get; set; } // Navigation property
    }
}
