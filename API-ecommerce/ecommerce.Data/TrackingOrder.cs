using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class TrackingOrder
    {
        [Key]
        public int id { get; set; }
        public string orderId { get; set; }
        public TrackingStatus trackingStatus { get; set; }
        public DateTime date { get; set; }
        public string userId { get; set; }
        public DateTime updateDate { get; set; }
        public string courierTrackingNumber { get; set; } 
        public string curierCopmany { get; set; }
        public string trackingUrl { get; set; }
        public string email { get; set; }
        public DateTime expectedArrival { get; set; } 
    } 
}
