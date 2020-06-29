using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class ProductType
    {
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; } // тип продукта (овощи, фрукты, напитки, мясо...)
    }
}
