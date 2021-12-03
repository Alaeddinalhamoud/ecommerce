using System;

namespace ecommerce.Dashboard.Models
{
    public class MVCouponNumberOfUseDetails
    {
        public int couponId { get; set; }
        public string couponCode { get; set; }
        public int numberOfUse { get; set; }
        public DateTime Date { get; set; }
    }
}
