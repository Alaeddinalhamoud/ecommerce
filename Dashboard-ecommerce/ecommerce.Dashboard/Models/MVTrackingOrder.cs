using ecommerce.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MVTrackingOrder
    {
        public int trackingId { get; set; }
        public string orderId { get; set; }
        public TrackingStatus trackingStatus { get; set; }   
        public string courierTrackingNumber { get; set; }
        public string curierCopmany { get; set; }
        [DataType(DataType.Url)]
        public string trackingUrl { get; set; }
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [DataType(DataType.Date)]
        public DateTime expectedArrivalDate { get; set; }
    }
}
