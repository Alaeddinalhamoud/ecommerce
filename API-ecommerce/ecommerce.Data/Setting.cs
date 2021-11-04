using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class Setting
    {
        [Key]
        public int id { get; set; }
        public string websiteName { get; set; }
        public int numberOfSliders { get; set; }
        public int pageSize { get; set; }
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [DataType(DataType.EmailAddress)]
        public string salesEmail { get; set; }
        [DataType(DataType.EmailAddress)]
        public string helpDeskEmail { get; set; }
        public bool isSlider { get; set; }
        public bool isMostViewdProduct { get; set; }
        public bool isNewArrivalProduct { get; set; }
        public bool isRecentlyViewedProduct { get; set; }
        public bool isTidioScript { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime createDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime updateDate { get; set; }
        [DataType(DataType.ImageUrl)]
        public string logo { get; set; }
        public string description { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string phone { get; set; }
        public string address { get; set; }
        [DataType(DataType.Html)]
        public string tidioScript { get; set; }
        [DataType(DataType.Currency)]
        public double tax { get; set; }
        [DataType(DataType.Currency)]
        public double shippingCost { get; set; }
        public bool isCash { get; set; }
        public bool isCard { get; set; }
        public bool enableMaintenance { get; set; }
        public bool isVendorEnabled { get; set; }
        [DataType(DataType.Url)]
        public string vendorAgreementContract { get; set; }
        public int payTabsMerchantProfileID { get; set; }
        [DataType(DataType.Password)]
        public string payTabsServerKey { get; set; }
        [DataType(DataType.Password)]
        public string payTabsClientKey	 { get; set; }
        public string payTabsAPIUrl { get; set; }
        public int orderReturnDays { get; set; }
        public bool enableCoupon { get; set; }
        public string facebookLink { get; set; }
        public string twitterLink { get; set; }
        public string instgramLink { get; set; }
    }
}
