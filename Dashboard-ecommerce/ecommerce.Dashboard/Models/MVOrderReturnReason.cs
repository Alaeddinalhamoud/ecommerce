using System.ComponentModel;

namespace ecommerce.Dashboard.Models
{
    public class MVOrderReturnReason
    {
        public int id { get; set; }
        [DisplayName("Reason")]
        public string reason { get; set; }
    }
}
