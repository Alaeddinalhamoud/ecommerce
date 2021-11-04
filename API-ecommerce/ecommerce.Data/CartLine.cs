using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class CartLine
    {
        [Key]
        public int id { get; set; }
        public int cartId { get; set; }
        [Display(Name = "Product")]       
        public int productId { get; set; }
        [Display(Name = "Quantity")]
        public int qty { get; set; } 
        public double total { get; set; }
        public bool freeTax { get; set; }
        public bool freeShipping { get; set; }
        [Display(Name = "Price")]
        public double price { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public string createdBy { get; set; }
    }
}
