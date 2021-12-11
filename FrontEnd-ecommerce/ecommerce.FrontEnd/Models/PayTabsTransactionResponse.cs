namespace ecommerce.FrontEnd.Models
{
    public class PayTabsTransactionResponse
    {
       public string tranRef { get; set; }
        public string cartId { get; set; }
        public string respStatus { get; set; }
        public int respCode { get; set; }
        public string respMessage { get; set; }
        public string acquirerRRN { get; set; }
        public string acquirerMessage { get; set; }
        public string token { get; set; }
        public string customerEmail { get; set; }
        public string signature { get; set; }
    }
}
