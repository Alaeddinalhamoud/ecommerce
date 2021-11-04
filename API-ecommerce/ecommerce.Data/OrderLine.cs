using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
  public  class OrderLine
    {
        [Key]
        public int id { get; set; }
        public string orderId { get; set; }
        [Display(Name = "Product")]
        public int productId { get; set; }
        [Display(Name = "Quantity")]
        public int qty { get; set; }
        [Display(Name = "Price")]
        public double price { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
        public bool freeTax { get; set; }
        public bool freeShipping { get; set; }
    }
}
