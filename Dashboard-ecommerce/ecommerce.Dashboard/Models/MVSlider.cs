using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MVSlider
    { 
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        public string description { get; set; }
        public bool updatelogo { get; set; }
        public string link { get; set; }
        public string buttonCaption { get; set; }
        public bool isEnabled { get; set; }
    }
}
