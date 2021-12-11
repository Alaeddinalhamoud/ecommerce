using ecommerce.AzUtilityFunctions.Email.Service;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.Email
{
    public class EmailsQueue
    {
        [FunctionName("EmailsQueue")]
        public async Task Run(
            [QueueTrigger("emails-submit", Connection = "AzureWebJobsStorage")] string myEmailItem,
            [Blob("emails-submit", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer emailSubmitContainer,
            [Blob("emails-archive", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer emailArchiveContainer,
            ILogger log)
        {
            log.LogInformation($" Queue trigger function processed: {myEmailItem}");
            CloudBlockBlob emailSubmit;
            CloudBlockBlob emailArchive;
            EmailSenderRequest emailContent = null;
            try
            {
                await emailArchiveContainer.CreateIfNotExistsAsync();
                await emailSubmitContainer.CreateIfNotExistsAsync();

                emailSubmit = emailSubmitContainer.GetBlockBlobReference(myEmailItem);
                emailArchive = emailArchiveContainer.GetBlockBlobReference(myEmailItem);
                emailContent = JsonSerializer.Deserialize<EmailSenderRequest>(await emailSubmit.DownloadTextAsync());
                //Use it with papercut
                if (Debugger.IsAttached)
                {
                    EmailLocalSender.LocalSender(emailContent);
                }
                else
                {
                    if (Environment.GetEnvironmentVariable("USE_SENDER").Equals("ZOHO"))
                    {
                        EmailSMTPSender.SMTPSender(emailContent);
                    }
                    else
                    {
                        await EmailSendGridSender.SendGridSender(log, emailContent);
                    }
                    await emailArchive.StartCopyAsync(emailSubmit);
                    await emailSubmit.DeleteAsync();
                    log.LogInformation($"Email has been sent to Archive {emailContent.to} , subject {emailContent.subject}");
                }
            }
            catch (SmtpException smtpEx)
            {
                log.LogError($" Smtp Exception Error while sending : {myEmailItem} , {smtpEx.ToString()}");
                await EmailSendGridSender.SendGridSender(log, emailContent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error while sending : {myEmailItem} , {ex.ToString()}");
            }
        }
    }
}
