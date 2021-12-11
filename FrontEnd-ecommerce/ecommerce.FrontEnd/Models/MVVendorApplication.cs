using ecommerce.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Models
{
    public class MVVendorApplication
    {
        public int id { get; set; }
        [Required(ErrorMessage = "IBAN Required")]
        public string iBAN { get; set; }
        [Required(ErrorMessage = "swiftCode Required")]
        public string swiftCode { get; set; }
        [Required(ErrorMessage = "account Required")]
        public string account { get; set; }
        [Required(ErrorMessage = "bank Address Required")]
        public string bankAddress { get; set; }
        [Required(ErrorMessage = "bank Name Required")]
        public string bankName { get; set; }
        [Required(ErrorMessage = "company Address Required")]
        public string companyAddress { get; set; }
        public string note { get; set; }
        [Required(ErrorMessage = "owner Id Required")]
        public string ownerIdNumber { get; set; }
        [Required(ErrorMessage = "CR Required")]
        public string crNumber { get; set; }
        public string tel2 { get; set; } 
        public string tel1 { get; set; }
        [Required(ErrorMessage = "Email Required")]
        [DataType(DataType.EmailAddress)]
        public string workEmail { get; set; }
        [Required(ErrorMessage = "VAT Required")]
        public string companyVAT { get; set; }
        [Required(ErrorMessage = "Company Name Required")]
        public string companyName { get; set; }
        [Required(ErrorMessage = "full Name Required")]
        public string fullName { get; set; }
        public Status status { get; set; }
        [DataType(DataType.Url)]
        public string vendorAgreementContract { get; set; }
        public IEnumerable<IFormFile> fileToUpload { get; set; }
    }
}
