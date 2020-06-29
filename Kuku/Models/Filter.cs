using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class Filter
    {
        public int itemId { get; set; }
        public string itemName { get; set; }
        public string itemType { get; set; }
        public int itemCount { get; set; }
        public int itemSort { get; set; }

        public string itemLink { get; set; }
        public string itemChecked { get; set; }
    }
}
