using System;

namespace ecommerce.Dashboard.Models
{
    public class MVCouponHistoryDetails
    {
        public int couponId { get; set; }
        public string couponCode { get; set; }
        public string orderId { get; set; }
        public DateTime Date { get; set; }
    }
}
