using Kuku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.ViewModels
{
    public class NationalCuisineListViewModel
    {
        public IEnumerable<NationalCuisine> NationalCuisines { get; set; }
        public string Name { get; set; }
        public Recipe Recipe { get; set; }
    }
}
