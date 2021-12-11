using System.Net.Mail;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.Email.Service
{
    public static class EmailLocalSender
    {
        //Debuger Sender
        public static void LocalSender(EmailSenderRequest emailContent)
        {
            SmtpClient smtpClient = new SmtpClient("127.0.0.1", 25);
            smtpClient.EnableSsl = false;
            smtpClient.UseDefaultCredentials = true;
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(emailContent.to));
            message.From = new MailAddress(emailContent.from);
            message.Subject = emailContent.subject;
            message.Body = emailContent.htmlContent;
            message.IsBodyHtml = true;
            smtpClient.Send(message);
        }
    }
}