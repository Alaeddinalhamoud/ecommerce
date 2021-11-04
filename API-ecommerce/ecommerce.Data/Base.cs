using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class Base
    {
        [Key]
        public int id { get; set; }
        public DateTime createDate { get; set; }
        public DateTime updateDate { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
    }
}
