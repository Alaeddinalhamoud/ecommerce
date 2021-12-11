using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Models
{
    public class MVFeedBackForm
    {
        [Required]
        public string name { get; set; }
        [DataType(DataType.EmailAddress), Required]
        public string email { get; set; }
        [DataType(DataType.Text), Required]
        public string subject { get; set; }
        [DataType(DataType.MultilineText), Required]
        public string message { get; set; }
        public string token { get; set; }
    }
}
