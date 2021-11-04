using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class OrderReturnReason
    {
        public int id { get; set; }
        public string reason { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
    }
}
