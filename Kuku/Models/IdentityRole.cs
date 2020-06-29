using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Kuku.Models
{
    public class IdentityRole
    {
        public virtual IKey Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }
        public virtual string ConcurrencyStamp { get; set; }
    }
}
