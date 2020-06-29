using Kuku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.ViewModels
{
    public class RecipeViewModel
    {
        public Recipe Recipes { get; set; }
        public IEnumerable<RecipeDetail> RecipesDetails { get; set; }

        public IEnumerable<Recipe_Product> Recipe_Products { get; set; }
        public List<Product> Products { get; set; }

        public IEnumerable<Recipe_TypeOfDish> Recipe_TypeOfDishes { get; set; }
        public List<TypeOfDish> TypeOfDishes { get; set; }

        public IEnumerable<Recipe_NationalCuisine> Recipe_NationalCuisenes { get; set; }
        public List<NationalCuisine> NationalCuisines { get; set; }

        public string AspNetUserName;

        public PageInfo PageInfo { get; set; }

        //public List<MeasuringSystem> MeasuringSystems { get; set; }
    }

}