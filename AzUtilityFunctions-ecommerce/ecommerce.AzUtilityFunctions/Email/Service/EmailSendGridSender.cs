using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.Email.Service
{
    public static class EmailSendGridSender
    {
        //Using SendGrid to send email
        public static async Task SendGridSender(ILogger log, EmailSenderRequest emailContent)
        {
            var client = new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_APIKEY"));
            var from = new EmailAddress(emailContent.from, emailContent.senderName);
            var subject = emailContent.subject;
            var to = new EmailAddress(emailContent.to, emailContent.receiverName);
            var plainTextContent = emailContent.plainTextContent;
            var htmlContent = emailContent.htmlContent;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            if (!String.IsNullOrEmpty(emailContent.cc))
            {
                msg.AddCc(emailContent.cc);
            }
            Response response = await client.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                log.LogError($"SendGrid Email sent failure to {emailContent.to} {response.Body.ToString()} , StatusCode: {response.StatusCode.ToString()}.");
                throw new Exception($"SendGrid Email sent failure to {emailContent.to} {response.Body.ToString()} , StatusCode: {response.StatusCode.ToString()}.");
            }
        }
    }
}
