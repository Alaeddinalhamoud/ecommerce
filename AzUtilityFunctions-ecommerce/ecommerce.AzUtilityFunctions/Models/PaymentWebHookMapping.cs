
using Microsoft.Azure.Cosmos.Table;

namespace ecommerce.AzUtilityFunctions.Models
{
    public class PaymentWebHookMapping : TableEntity
    {
        public string TranRef { get; set; }
        public string CartId { get; set; }
        public string RespStatus { get; set; }
        public int RespCode { get; set; }
        public string RespMessage { get; set; }
        public string AcquirerRRN { get; set; }
        public string AcquirerMessage { get; set; }
        public string CustomerEmail { get; set; }
        public string Signature { get; set; }
    }
}
