using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class Slider
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        [DataType(DataType.ImageUrl)]
        public string imagePath { get; set; }
        [DataType(DataType.Url)]
        public string link { get; set; }
        public string buttonCaption { get; set; }
        public bool isEnabled { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime createDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime updateDate { get; set; }
    }
}
