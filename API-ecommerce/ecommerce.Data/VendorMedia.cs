using System;
using System.ComponentModel.DataAnnotations;
namespace ecommerce.Data
{
    public class VendorMedia
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Media Name")]
        public string name { get; set; }
        [Display(Name = "Media Type")]
        public MediaType mediaType { get; set; }
        public string alt { get; set; }
        public string path { get; set; }
        public string vendorId { get; set; } 
        public string createdBy { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
    }

   
}
