using ecommerce.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Dashboard.Models
{
    public class MVOrderComplaintCase
    {
        public int id { get; set; }
        [Required, Display(Name = "Order Id")]
        public string orderId { get; set; }
        [Display(Name = "Reason")]
        public int reasonId { get; set; }
        public SelectList reasons { get; set; }
        [Display(Name = "Customer note")]
        public string customerNote { get; set; }
        [Display(Name = "Status")]
        public Status status { get; set; }
        [Display(Name = "Customer Email")]
        public string email { get; set; }
        [Display(Name = "Actions to solve")]
        public string actionsToSolve { get; set; }
        [Display(Name = "Message to the customer")]
        public string messageToCustomer { get; set; }
        public bool sendEmail { get; set; }
    }
}
