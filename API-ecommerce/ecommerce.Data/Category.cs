using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class Category
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Category Name")]
        public string name { get; set; }
        public string createdBy { get; set; }
        [Display(Name ="Category Image")]
        [DataType(DataType.ImageUrl)]
        public string imagePath { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
