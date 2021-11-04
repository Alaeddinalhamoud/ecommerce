using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class Review
    {
        [Key]
        public int id { get; set; }
       
        [Display(Name ="User Name")]
        public string createdBy { get; set; }
        [Display(Name = "Rating")]
        public int rating { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int productId { get; set; }
      
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
        public bool isApproved { get; set; }
    }
}
