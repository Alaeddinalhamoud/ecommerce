using ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Dashboard.Models
{
    public class MVCoupon:Base
    {
        [Required]
        public string couponName { get; set; }
        [Remote(action: "IsExistCouponCode", controller: "Coupon")]
        public string code { get; set; }
        [Required]
        public DiscountType discountType { get; set; }
        public int value { get; set; }
        public int numberOfUse { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime startOn { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime expireOn { get; set; }
        public int minimumSpent { get; set; }
        public int maximumDiscount { get; set; }
        public bool isActive { get; set; }
    }
}
