using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kuku.Models
{
    public class Recipe_Product
    {
        //internal Recipe_Product Recipe_product;

        //[Key, ForeignKey("Recipe")]
        //[Column(Order = 1)]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        //[Key, ForeignKey("Product")]
        //[Column("ProductId", Order = 0)]
        //[Key]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public float Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
