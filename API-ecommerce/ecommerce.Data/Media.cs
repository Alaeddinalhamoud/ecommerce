using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class Media
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Media Name")]
        public string name { get; set; }
        [Display(Name = "Media Type")]
        public MediaType mediaType { get; set; }
        public string alt { get; set; }
        public string path { get; set; }
        [Display(Name = "Product Name")]
        public int productId { get; set; } 
        [Display(Name = "User Name")]
        public string createdBy { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
    }

   
}
