using Kuku.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.ViewModels
{
    public class ProductsListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public SelectList ProductTypes { get; set; }
        public string Name { get; set; }
        public Recipe Recipe { get; set; }
        public float Quantity { get; set; }
    }
}
