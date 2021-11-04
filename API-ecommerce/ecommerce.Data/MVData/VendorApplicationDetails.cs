using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data.MVData
{
   public class VendorApplicationDetails
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string companyName { get; set; }
        public string  companyVAT { get; set; }
        [DataType(DataType.EmailAddress)]
        public string workEmail { get; set; }
        public string tel1 { get; set; }
        public string tel2 { get; set; }
        public string crNumber { get; set; }
        public string ownerIdNumber { get; set; }
        public string note { get; set; }
        public string companyAddress { get; set; }
        public string bankName { get; set; }
        public string bankAddress { get; set; }
        public string account { get; set; }
        public string swiftCode { get; set; }
        public string iBAN { get; set; }
        public Status status { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
        //Medias
        public IEnumerable<VendorMedia> vendorMedias { get; set; }
    }
}
