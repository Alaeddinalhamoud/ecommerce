using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data.MVData
{
   public class VendorProfileDetails
    { 
        public int VendorProfileId { get; set; }
        public string companyName { get; set; }
        public string companyVAT { get; set; }
        [DataType(DataType.EmailAddress)]
        public string workEmail { get; set; }
        public string tel1 { get; set; }
        public string tel2 { get; set; }
        public string crNumber { get; set; }
        public string ownerIdNumber { get; set; }
        public string companyAddress { get; set; }
        public Status status { get; set; }
        //Bank
        public int bankId { get; set; }
        public string bankName { get; set; }
        public string bankAddress { get; set; }
        public string account { get; set; }
        public string swiftCode { get; set; }
        public string iBAN { get; set; }
        //Medias
        public IEnumerable<VendorMedia> vendorMedias { get; set; }
    }
}
