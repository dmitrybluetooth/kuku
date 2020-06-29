using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class RecipeDetail
    {
        public int RecipeDetailId { get; set; }
        public int RecipeId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OriginalImageId { get; set; }
        public byte[] PreviewImageData { get; set; }
        public byte[] BigImageData { get; set; }

        public Recipe Recipe { get; set; }
    }
}
