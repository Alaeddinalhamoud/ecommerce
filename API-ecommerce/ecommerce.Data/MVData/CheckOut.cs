using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data.MVData
{
   public class CheckOut
    {
        public string userId { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int cartId { get; set; }
        public string orderId { get; set; }
        public bool  isPaid { get; set; }
        public double discount { get; set; }
        public double total { get; set; }
        public double totalTaxedProduct { get; set; }
        public double totalFreeTaxProduct { get; set; }
        public Status status { get; set; }
        public int addressId { get; set; }     
        public string createdBy { get; set; }   
        public DateTime createDate { get; set; }      
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; } 
        public string token { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public double tax { get; set; }
        public double taxedCost { get; set; }
        [DataType(DataType.Currency)]
        public double shippingCost { get; set; }
        [DataType(DataType.Currency)]
        public double subTotal { get; set; }
        public string paymentReference { get; set; }
        public bool isCash { get; set; }
        public bool isCard { get; set; }
        public bool isSTCPay { get; set; }
        public string sTCPayQR { get; set; }
        public string couponCode { get; set; }
        public int couponValue { get; set; }
        public DiscountType discountType { get; set; }
        public string couponName { get; set; }
        public bool couponEnabled { get; set; }
        public int couponNumberOfUse { get; set; }
        public DateTime couponStartOn { get; set; }
        public DateTime couponExpireOn { get; set; }
        public int couponMinimumSpend { get; set; }
        public int couponMaximumDiscount { get; set; }
        public string couponHidden { get; set; }
    }
}
