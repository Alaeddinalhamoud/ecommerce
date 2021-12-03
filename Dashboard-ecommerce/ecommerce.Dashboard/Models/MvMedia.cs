using ecommerce.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MvMedia
    {
        public int id { get; set; }
        [Required]
        public string alt { get; set; }
        [Required]
        public string name { get; set; }
        public string path { get; set; }
        public int productId { get; set; }
        public MediaType mediaType { get; set; }
    }
}
