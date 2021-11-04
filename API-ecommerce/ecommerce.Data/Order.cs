using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class Order
    {
        [Key]
        public string id { get; set; } 
        [Display(Name ="User Name")]
        public string createdBy { get; set; } 
        [Display(Name = "Description")]
        public string description { get; set; }
        public TrackingStatus trackingStatus { get; set; }
        [Display(Name = "Total")]
        public double total { get; set; }
        [Display(Name = "Is Paide")]
        public bool isPaid { get; set; }
        [Display(Name = "Payment Method")]
        public PaymentMethod paymentMethod { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
        public int cartId { get; set; }
        public double tax { get; set; }
        public double taxCost { get; set; }
        [DataType(DataType.Currency)]
        public double shippingCost { get; set; }
        [DataType(DataType.Currency)]
        public double subTotal { get; set; }
        [DataType(DataType.Currency)]
        public double discount { get; set; }
        public Status status { get; set; }
    }
}
