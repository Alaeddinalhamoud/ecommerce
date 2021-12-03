using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MvBrand
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        public bool updateLogo { get; set; }
    }
}
