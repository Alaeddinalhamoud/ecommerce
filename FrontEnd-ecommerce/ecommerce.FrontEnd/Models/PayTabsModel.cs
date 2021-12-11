using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ecommerce.FrontEnd.Models
{
    public class PayTabsModel
    {
    }

    public class PaytabsRequest
    {
        [JsonPropertyName("profile_id")]
        public int PofileId { get; set; }
        [JsonPropertyName("tran_type")]
        public string TransType { get; set; }
        [JsonPropertyName("tran_class")]
        public string TransClass { get; set; }
        [JsonPropertyName("cart_id")]
        public string CartId { get; set; }
        [JsonPropertyName("cart_description")]
        public string CartDescription { get; set; }
        [JsonPropertyName("cart_currency")]
        public string CartCurrency { get; set; }
        [JsonPropertyName("cart_amount")]
        public double CartAmount { get; set; }
        [JsonPropertyName("callback")]
        public string Callback { get; set; }
        [JsonPropertyName("return")]
        public string Return { get; set; }
        [JsonPropertyName("customer_details")]
        public CustomerDetails CustomerDetails { get; set; }
        [JsonPropertyName("hide_shipping")]
        public bool HideShipping { get; set; }
    }

   public class CustomerDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
        [JsonPropertyName("street1")]
        public string Street { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
        [JsonPropertyName("country")]
        public string Country { get; set; }
        [JsonPropertyName("ip")]
        public string IP { get; set; }
    }

public class PayTabsResponse
    {
        [JsonPropertyName("redirect_url")]
        public string RedirectUrl { get; set; }
        [JsonPropertyName("tran_ref")]
        public string TransRef { get; set; }
        [JsonPropertyName("tran_type")]
        public string TransType { get; set; }
        [JsonPropertyName("tran_class")]
        public string TransClass { get; set; }
        [JsonPropertyName("cart_id")]
        public string CartId { get; set; }
        [JsonPropertyName("cart_description")]
        public string CartDescription { get; set; }
        [JsonPropertyName("cart_currency")]
        public string CartCurrency { get; set; }
        [JsonPropertyName("cart_amount")]
        public string CartAmount { get; set; }
        [JsonPropertyName("callback")]
        public string Callback { get; set; }
        [JsonPropertyName("return")]
        public string Return { get; set; }
    }
}
