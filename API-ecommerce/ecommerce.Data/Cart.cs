using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class Cart
    {
        [Key]
        public int id { get; set; } 
        public string createdBy { get; set; } 
        public Status status { get; set; }       
        [DataType(DataType.DateTime)]
        public DateTime? createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
        public int couponId { get; set; }
    } 
}
