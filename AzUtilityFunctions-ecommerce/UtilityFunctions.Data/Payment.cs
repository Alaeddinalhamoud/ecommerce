
namespace UtilityFunctions.Data
{
    class Payment
    {
    }

    public class PaymentResponse
    {
        public bool flag { get; set; }
        public string message { get; set; }
    }
    //userID, userName, OrderId, AddressId, Token, Total, paymentMethod(by defualt card
    public class PaymentRequest
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
        public string emailBody { get; set; }
    }

    public enum PaymentMethod
    {
        Card = 1,
        Cash = 2,
        StcPay = 3
    }
}
