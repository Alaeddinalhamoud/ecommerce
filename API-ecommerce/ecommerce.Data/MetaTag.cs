using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class MetaTag
    {
        [Key]
        public int id { get; set; }
        public int productId { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string image { get; set; }
        public string imageAlt { get; set; }
        public string url { get; set; }
        public string locale { get; set; }
        public string sitename { get; set; }
        public string video { get; set; }
        public string keywords { get; set; }
        public MetaTagType metaTagType { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
    }
}
