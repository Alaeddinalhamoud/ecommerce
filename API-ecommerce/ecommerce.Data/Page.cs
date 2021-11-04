using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class Page
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        [DataType(DataType.Html)]
        public string content { get; set; }
        public Navs nav { get; set; }
        public bool published { get; set; }
        public bool isPrivate { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
    }
}
