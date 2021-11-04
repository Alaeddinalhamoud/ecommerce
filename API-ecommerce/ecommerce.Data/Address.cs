using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class Address
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Full Name"), Required]
        public string fullName { get; set; }
        [Display(Name ="House Number"), Required]
        public string houseNumber { get; set; }
        [Display(Name = "Address Line1"), Required]
        public string addressline1 { get; set; }
        [Display(Name = "Address Line2")]
        public string addressline2 { get; set; }
        [Display(Name = "Street Name"), Required]
        public string street { get; set; }
        [Display(Name = "City"), Required]
        public string city { get; set; }
        [Display(Name = "ZIP Code"), Required]
        public string code { get; set; }
        [Display(Name = "Country"), Required]
        public string country { get; set; }
        //Here we need the UserTable to make the relationship       
        public string createdBy { get; set; }
        [Display(Name = "Longitude Number")]
        public double longitude { get; set; }
        [Display(Name = "Latitude")]
        public double latitude { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }

    }
}
