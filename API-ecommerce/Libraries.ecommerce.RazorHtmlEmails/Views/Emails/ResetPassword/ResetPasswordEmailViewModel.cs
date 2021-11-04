namespace Libraries.ecommerce.RazorHtmlEmails.Views.Emails.ResetPassword
{
    public class ResetPasswordEmailViewModel
    {
        public ResetPasswordEmailViewModel(string resetPasswordEmailUrl)
        {
            ResetPasswordEmailUrl = resetPasswordEmailUrl;
        }

        public string ResetPasswordEmailUrl { get; set; }
    }
}
