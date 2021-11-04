using System;

namespace ecommerce.Data
{
    public class Coupon:Base
    {
        public string couponName { get; set; }
        public string code { get; set; }
        public DiscountType discountType { get; set; } 
        public int value { get; set; }
        public int numberOfUse { get; set; }
        public DateTime startOn { get; set; }
        public DateTime expireOn { get; set; }
        public int minimumSpend { get; set; }
        public int maximumDiscount { get; set; }
        public bool isActive { get; set; }
    }
}
