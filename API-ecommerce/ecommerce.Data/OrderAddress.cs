using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class OrderAddress
    {
        [Key]
        public int id { get; set; }
        public string orderId { get; set; }
        [Display(Name ="House Number")]
        public string houseNumber { get; set; }
        [Display(Name = "Address Line1")]
        public string addressline1 { get; set; }
        [Display(Name = "Address Line2")]
        public string addressline2 { get; set; }
        [Display(Name = "Street Name")]
        public string street { get; set; }
        [Display(Name = "City")]
        public string city { get; set; }
        [Display(Name = "Code Number")]
        public string code { get; set; }
        [Display(Name = "Country")]
        public string country { get; set; }
        //Here we need the UserTable to make the relationship       
        public string createdBy { get; set; }
        [Display(Name = "Longitude Number")]
        public double longitude { get; set; }
        [Display(Name = "Latitude")]
        public double latitude { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
    }
}
