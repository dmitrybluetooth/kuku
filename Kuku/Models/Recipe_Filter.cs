using System.Collections.Generic;

namespace Kuku.Models
{
    public class Recipe_Filter
    {
        public string itemType { get; set; }

        public string itemMD5;
        public string itemClass;

        public int itemsCount;
        public int itemSort { get; set; }

        public List<Filter> items { get; set; }
 
    }
}