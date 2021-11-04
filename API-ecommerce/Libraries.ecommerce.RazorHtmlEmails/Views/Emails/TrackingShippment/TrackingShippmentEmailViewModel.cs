using ecommerce.Data;
using System;

namespace Libraries.ecommerce.RazorHtmlEmails.Views.Emails.TrackingShippment
{
    public class TrackingShippmentEmailViewModel
    {
        public TrackingShippmentEmailViewModel(string orderId, string courierTrackingNumber,
            string courierCopmany, TrackingStatus trackingStatus, DateTime expectedArrival,
            string trackingLink)
        {
            OrderId = orderId;
            CourierCopmany = courierCopmany;
            CourierTrackingNumber = courierTrackingNumber;
            TrackingStatus = trackingStatus;
            ExpectedArrival = expectedArrival;
            TrackingLink = trackingLink;
        }

        public string OrderId { get; set; }
        public string CourierTrackingNumber { get; set; }
        public string CourierCopmany { get; set; }
        public TrackingStatus TrackingStatus { get; set; }
        public DateTime ExpectedArrival { get; set; }
        public string TrackingLink { get; set; }
    }
}
