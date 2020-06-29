using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class SP_Recipe
    {
        public string FileName { get; set; }
        public byte[] OriginalImageData { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public byte[] BigImageData { get; set; }
        public byte[] PreviewImageData { get; set; }
        public string UserId { get; set; }
    }
}
