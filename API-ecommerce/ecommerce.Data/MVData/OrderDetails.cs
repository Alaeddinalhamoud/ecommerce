using System;
using System.Collections.Generic;

namespace ecommerce.Data.MVData
{
   public class OrderLineProduct
    {
       
        public int orderLineId { get; set; }
        public int qty { get; set; }        
        public double price { get; set; }        
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
        public string createdBy { get; set; }
        public int productId { get; set; }
        public string prodcutName { get; set; }
        public string productImage { get; set; } = "/assets/img/ZeroData.svg";
        public bool freeTax { get; set; }
        public bool freeShipping { get; set; }
    }

    public class OrderDetails 
    {
        public string orderId { get; set; }
        public string FullName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public bool isPaid { get; set; }
        public string paymentTransaction { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public TrackingStatus trackingStatus { get; set; }
        public OrderAddress orderAddress { get; set; }
        public double subTotal { get; set; }
        public double shippingCost { get; set; }
        public double tax { get; set; }
        public double taxCost { get; set; }
        public double discount { get; set; }
        public double total { get; set; } 
        public DateTime createDate { get; set; }
        public Status Status { get; set; }
        public string createdBy { get; set; }
        public TrackingOrder trackingOrder { get; set; }

        //Order
        public int orderReturnDays { get; set; }
        public List<OrderLineProduct> orderLineProducts { get; set; } = new List<OrderLineProduct>();
    }
}
