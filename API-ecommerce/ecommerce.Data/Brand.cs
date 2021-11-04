using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
  public  class Brand
    {
        [Key]
        public int id { get; set; } 
        public string name { get; set; } 
        public string createdBy { get; set; } 
        [DataType(DataType.ImageUrl)]
        public string imagePath { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }

    }
}
