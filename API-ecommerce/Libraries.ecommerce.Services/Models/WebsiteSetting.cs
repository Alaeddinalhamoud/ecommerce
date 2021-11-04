namespace Libraries.ecommerce.Services.Models
{
    public class WebsiteSetting
    {
        public string APIUrl { get; set; }
        public string FileUploadUrl { get; set; }
        public string FileUploadAPIKey { get; set; }
        public string EmailSenderUrl { get; set; }
        public string EmailSenderAPIKey { get; set; }
        public string APIAuthUrl { get; set; }
        public string PaymentUrl { get; set; }
        public string PaymentAPIKey { get; set; }
        public string AuthServerSecret { get; set; }
    }
}
