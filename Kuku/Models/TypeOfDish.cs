using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class TypeOfDish
    {
        public int TypeOfDishId { get; set; }
        public string TypeOfDishName { get; set; } // тип блюда (первое, барбекю, салат, мясо...)    

        public List<Recipe_TypeOfDish> Recipe_TypeOfDishes { get; set; }
    }
}
