using Kuku.Models;
using System.Collections.Generic;

namespace Kuku.ViewModels
{

    public class FilterViewModel
    {
       
        public IEnumerable<Recipe> Recipes { get; set; }

        public IEnumerable<Recipe_Product> Recipe_Products { get; set; }
        public List<Filter> TopFilterProduct { get; set; }

        public List<Filter> Products { get; set; }

        public IEnumerable<Recipe_TypeOfDish> Recipe_TypeOfDishes { get; set; }
        public List<Filter> TypeOfDishes { get; set; }

        public IEnumerable<Recipe_NationalCuisine> Recipe_NationalCuisenes { get; set; }
        public List<Filter> NationalCuisines { get; set; }

        //public List<FilterProduct> FilterProducts { get; set; }
        //public List<FilterNationalCuisine> FilterNationalCuisines { get; set; }
        //public List<FilterTypeOfDish> FilterTypeOfDishes { get; set; }

        public List<Recipe_Filter> Recipe_Filters { get; set; }

        public PageInfo PageInfo { get; set; }


        //@model PagedList.IPagedList<PagingApp.Models.Recipe>

        public string flp;
        public string flc;
        public string fld;

        //public List<MeasuringSystem> MeasuringSystems { get; set; }
    }
}
