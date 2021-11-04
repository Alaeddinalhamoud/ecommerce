using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class ProductSpecification
    {
        [Key]
        public int id { get; set; }


        [Display(Name = "Product Name")]
        
        public int productId { get; set; }

        [Display(Name = "Name")]
      
        public string name { get; set; }

        [Display(Name = "Value")]
         
        public string value { get; set; }

        
        public string createdBy { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
