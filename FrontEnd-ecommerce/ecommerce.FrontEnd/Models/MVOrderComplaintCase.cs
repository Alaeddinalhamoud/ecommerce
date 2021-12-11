using ecommerce.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.FrontEnd.Models
{
    public class MVOrderComplaintCase
    {
        public int id { get; set; }
        [Required, Display(Name = "Order Id")]
        public string orderId { get; set; }
        [Required, Display(Name ="Reason")]
        public int reasonId { get; set; }
        public SelectList reasons { get; set; }
        [Required, Display(Name = "Description")]
        public string customerNote { get; set; }
        public Status status { get; set; }
        public string email { get; set; }
    }
}
