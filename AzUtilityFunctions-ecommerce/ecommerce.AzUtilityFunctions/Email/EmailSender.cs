using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.Email
{
    public static class EmailSender
    {
        [FunctionName("EmailSender")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("emails-submit", Connection = "AzureWebJobsStorage")] CloudQueue emailSubmitQueue,
            [Blob("emails-submit", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer emailSubmitContainer,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request Email Sender.");
            EmailSenderResponse emailSenderResponse = new EmailSenderResponse();
            if (req == null)
            {
                log.LogError("no data to send.");
                emailSenderResponse.flag = false;
                emailSenderResponse.message = "You have sent empty content.";
                return (ActionResult)new OkObjectResult(emailSenderResponse);
            }

            try
            {
                var fileName = $"{Guid.NewGuid().ToString()}.json";
                await emailSubmitContainer.CreateIfNotExistsAsync();
                CloudBlockBlob blob = emailSubmitContainer.GetBlockBlobReference(fileName);

                // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                //Send to te container
                EmailSenderRequest emailContent = await JsonSerializer.DeserializeAsync<EmailSenderRequest>(req.Body);
                await blob.UploadTextAsync(JsonSerializer.Serialize(emailContent));
                log.LogInformation($"Email sent to Blob {emailContent.to}");
                //Sent ot Queue
                var queueMessage = new CloudQueueMessage(fileName);
                await emailSubmitQueue.AddMessageAsync(queueMessage);
                log.LogInformation($"Email sent to Queue {emailContent.to}");
                emailSenderResponse.flag = true;
                emailSenderResponse.message = $"Email has been added to Queue.";

            }
            catch (Exception ex)
            {
                log.LogError($"error msg : {ex.ToString()}");
                emailSenderResponse.flag = false;
                emailSenderResponse.message = ex.ToString();
                return new BadRequestObjectResult(emailSenderResponse);
            }
            return (ActionResult)new OkObjectResult(emailSenderResponse);
        }
    }
}
