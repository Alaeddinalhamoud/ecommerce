using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Models
{
    public class PayTabsPaymentResponse
    {
       
        public string result { get; set; }
        [JsonPropertyName("response_code")]
        public string responseCode { get; set; }
        [JsonPropertyName("payment_url")]
        public string paymentUrl { get; set; }
        [JsonPropertyName("p_id")]
        public int paymentId { get; set; }
     }
}
