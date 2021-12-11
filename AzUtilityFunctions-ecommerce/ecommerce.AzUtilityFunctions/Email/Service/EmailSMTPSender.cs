using System;
using System.Net;
using System.Net.Mail;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.Email.Service
{
    public static class EmailSMTPSender
    {
        public static void SMTPSender(EmailSenderRequest emailContent)
        {
            SmtpClient smtpClient = new SmtpClient(Environment.GetEnvironmentVariable("SMTP_HOST"), 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("SMTP_EAMIL"),
                                                           Environment.GetEnvironmentVariable("SMTP_PASSWORD"));
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(emailContent.to, emailContent.receiverName));
            message.From = new MailAddress(emailContent.from, emailContent.senderName);
            if (!String.IsNullOrEmpty(emailContent.cc))
            {
                message.CC.Add(new MailAddress(emailContent.cc, emailContent.senderName));
            }
            message.Subject = emailContent.subject;
            message.Body = emailContent.htmlContent;
            message.IsBodyHtml = true;
            smtpClient.Send(message);
        }
    }
}
