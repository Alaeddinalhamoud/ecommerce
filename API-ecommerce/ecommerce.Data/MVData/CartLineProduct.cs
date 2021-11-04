using System;
using System.Collections.Generic;

namespace ecommerce.Data.MVData
{
    public class CartLineDetails
    {
        public int cartId { get; set; }
        public Status cartStatus { get; set; }
        public List<CartLineProduct> cartLineProduct { get; set; } = new List<CartLineProduct>();
        public double tax { get; set; }
        public bool isCard { get; set; }
        public bool isCash { get; set; }
        public double shippingCost { get; set; }
        public string couponCode { get; set; }
        public int couponValue { get; set; }
        public DiscountType  discountType { get; set; }
        public string couponName { get; set; }
        public bool couponEnabled { get; set; }
        public int couponNumberOfUse { get; set; }
        public DateTime couponStartOn { get; set; }
        public DateTime couponExpireOn { get; set; }
        public int couponMinimumSpend { get; set; }
        public int couponMaximumDiscount { get; set; }

    }
   public class CartLineProduct
    {
        public int cartLineId { get; set; }
        public int qty { get; set; }        
        public double price { get; set; }
        public double total { get; set; }
        public string createdBy { get; set; }
        public int productId { get; set; }
        public string prodcutName { get; set; }
        public string productImage { get; set; }
        public bool freeShipping { get; set; }
        public bool freeTax { get; set; }
    }
}
