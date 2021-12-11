using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Models
{
    public class MVReview
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        public int rating { get; set; }
        [Required]
        public string reviewerName { get; set; }
        [Required]
        public string reviewDescription { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string reviewerEmail { get; set; }
        public int productId { get; set; }
    }
}
