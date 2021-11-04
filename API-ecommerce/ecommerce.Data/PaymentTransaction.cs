using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class PaymentTransaction
    {
        [Key]
        public string id { get; set; }
        public string userId { get; set; }
        public string fullName { get; set; }
        public string orderId { get; set; }
        public double amount { get; set; }
        public string status { get; set; }
        public DateTime createDate { get; set; }
        public PaymentMethod paymentMethod { get; set; }
    }
}
