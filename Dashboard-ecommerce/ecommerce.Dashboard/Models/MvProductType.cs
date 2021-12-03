using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MvProductType
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }   
    }
}
