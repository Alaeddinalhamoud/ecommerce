using ecommerce.Data.MVData;
using System;
using System.Linq;

namespace ecommerce.Data.Helper
{
    public static class TotalCart
    {
        public static TotalCartModels GetTotalCart(CartLineDetails Model)
        {
            TotalCartModels totalCartModels = new TotalCartModels();
            totalCartModels.totalFreeTaxProduct = Model.cartLineProduct.Where(x => x.freeTax.Equals(true)).Sum(x => x.total);
            totalCartModels.totalTaxedProduct = Model.cartLineProduct.Where(x => x.freeTax.Equals(false)).Sum(x => x.total);
            totalCartModels.subtotal = totalCartModels.totalTaxedProduct + totalCartModels.totalFreeTaxProduct;
            totalCartModels.shippingCost = (Model.cartLineProduct.Any(x => x.freeShipping.Equals(false))) ? Model.shippingCost : 0.0f;
            totalCartModels.tax = (Model.cartLineProduct.Any(x => x.freeTax.Equals(false))) ? Model.tax : 0.0f;
            //Coupon check and calc
            totalCartModels.cartDiscount = 0.0f;
            totalCartModels.couponHidden = "d-none";
            if (Model.couponEnabled && Model.couponStartOn <= DateTime.Now.Date && Model.couponExpireOn >= DateTime.Now.Date && Model.couponMinimumSpend <= totalCartModels.subtotal)
            {
                totalCartModels.couponHidden = String.Empty;
                if (Model.discountType.Equals(DiscountType.FixedAmount))
                {
                    totalCartModels.cartDiscount = Model.couponValue;
                }
                else if (Model.discountType.Equals(DiscountType.Percentage))
                {
                    totalCartModels.cartDiscount = Math.Round((Model.couponValue / 100f) * totalCartModels.subtotal, 2);
                    if (totalCartModels.cartDiscount > Model.couponMaximumDiscount)
                    {
                        totalCartModels.cartDiscount = Model.couponMaximumDiscount;
                    }
                }
                else if (Model.discountType.Equals(DiscountType.FreeShipping))
                {
                    totalCartModels.cartDiscount = totalCartModels.shippingCost;
                }
            }
            totalCartModels.taxCost = totalCartModels.tax.Equals(0) ? 0.0f : ((totalCartModels.totalTaxedProduct) * (Model.tax / 100f));
            totalCartModels.total = totalCartModels.taxCost + totalCartModels.subtotal + totalCartModels.shippingCost - totalCartModels.cartDiscount;
            return totalCartModels;
        }
    }

    public class TotalCartModels
    {
        public double totalFreeTaxProduct { get; set; }
        public double totalTaxedProduct { get; set; }
        public double shippingCost { get; set; }
        public double cartDiscount { get; set; }
        public double taxCost { get; set; }
        public double tax { get; set; }
        public double total { get; set; }
        public double subtotal { get; set; }
        public string couponHidden { get; set; }
    }
}
