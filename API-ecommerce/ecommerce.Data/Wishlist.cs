using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class Wishlist
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Product Name")]
        public int productId { get; set; }
       
        [Display(Name = "created By")]
        public string createdBy { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        [Display(Name = "User")]
        public string userId { get; set; }
    }
}
