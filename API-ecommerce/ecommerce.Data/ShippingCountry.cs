using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class ShippingCountry
    {
        public int id { get; set; }
        public string country { get; set; }
        public string code { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
    }
}
