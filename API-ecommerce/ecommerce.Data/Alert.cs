using System;
using System.ComponentModel.DataAnnotations;
namespace ecommerce.Data
{
    public class Alert
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public AlertType alertType { get; set; }
        public bool isEnabled { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
    }
}
