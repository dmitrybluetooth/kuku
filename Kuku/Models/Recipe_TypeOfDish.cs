using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class Recipe_TypeOfDish
    {
        //[Key]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int TypeOfDishId { get; set; }
        public TypeOfDish TypeOfDish { get; set; }
    }
}
