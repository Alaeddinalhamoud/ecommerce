using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class VendorBank
    {
        [Key]
        public int id { get; set; }
        public int vendorProfileId { get; set; }
        public string bankName { get; set; }
        public string bankAddress { get; set; }
        public string account { get; set; }
        public string swiftCode { get; set; }
        public string iBAN { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
    }
}
