using System;

namespace UtilityFunctions.Data
{
    public class CustomerDetails
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string street1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string ip { get; set; }
    }

    public class PaymentResult
    {
        public string response_status { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string acquirer_message { get; set; }
        public string acquirer_rrn { get; set; }
        public DateTime transaction_time { get; set; }
    }

    public class PaymentInfo
    {
        public string card_type { get; set; }
        public string card_scheme { get; set; }
        public string payment_description { get; set; }
    }

    public class PayTabsVerifyPaymentResponse
    {
        public string tran_ref { get; set; }
        public string cart_id { get; set; }
        public string cart_description { get; set; }
        public string cart_currency { get; set; }
        public string cart_amount { get; set; }
        public CustomerDetails customer_details { get; set; }
        public PaymentResult payment_result { get; set; }
        public PaymentInfo payment_info { get; set; }
    }
}
