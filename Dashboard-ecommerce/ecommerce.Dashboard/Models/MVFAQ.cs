using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MVFAQ
    {
        public int id { get; set; }
        [Required]
        public string question { get; set; }
        [DataType(DataType.Html)]
        public string answer { get; set; }
    }
}
