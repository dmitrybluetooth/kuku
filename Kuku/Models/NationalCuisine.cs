using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class NationalCuisine
    {
        public int NationalCuisineId { get; set; }
        public string NationalCuisineName { get; set; } // название национальной кухни       

        public List<Recipe_NationalCuisine> Recipe_NationalCuisines { get; set; }
    }
}
