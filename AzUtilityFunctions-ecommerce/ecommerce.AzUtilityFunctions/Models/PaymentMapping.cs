using Microsoft.Azure.Cosmos.Table;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.Models
{
    public class PaymentMapping : TableEntity
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string orderId { get; set; }
        public string addressId { get; set; }
        public string transactionResponseCode { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        // 1 for card 2 for cash
        public PaymentMethod paymentMethod { get; set; }
        public string paymentReference { get; set; }
        public string emailFileName { get; set; }
        public int executionCount { get; set; }
    }
}
