using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class OriginalImage
    {
        public int OriginalImageId { get; set; }
        //public int RecipeId { get; set; }
        public string FileName { get; set; }
        //public string Title { get; set; }
        public byte[] OriginalImageData { get; set; }
    }
}
