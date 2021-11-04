using ecommerce.Data;
using ecommerce.Data.MVData;

namespace Libraries.ecommerce.RazorHtmlEmails.Views.Emails.Order
{
    public class OrderEmailViewModel
    {
        public OrderEmailViewModel(string name, string orderId, string total, OrderDetails orderDetails,
            Address address, string subTotal, string shippingCost, string tax, string taxCost)
        {
            Name = name;
            OrderId = orderId;
            Total = total;
            OrderDetails = orderDetails;
            Address = address;
            Tax = tax;
            TaxCost = taxCost;
            ShippingCost = shippingCost;
            SubTotal = subTotal;
        }

        public string Name { get; set; }
        public string OrderId { get; set; }
        public string SubTotal { get; set; } 
        public string ShippingCost { get; set; }
        public string Tax { get; set; }
        public string TaxCost { get; set; }
        public string Total { get; set; }
        public OrderDetails OrderDetails { get; set; }
        public Address Address { get; set; }
    }
}
