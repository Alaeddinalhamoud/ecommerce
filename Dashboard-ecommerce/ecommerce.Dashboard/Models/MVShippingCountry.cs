using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Dashboard.Models
{
    public class MVShippingCountry
    {
        public int id { get; set; }
        [DisplayName("Country")]
        [Required, Remote(action: "IsNewCountry", controller: "ShippingCountry")]
        public string country { get; set; }
        [Required, DisplayName("Code")]
        public string code { get; set; }
    }
}
