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

namespace ecommerce.AzUtilityFunctions.PaymentGateway
{
    public static class Payment
    {
        [FunctionName("Payment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("payments-submit", Connection = "AzureWebJobsStorage")] CloudQueue paymentSubmitQueue,
            [Blob("payments-submit", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentSubmitContainer,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request Payment.");
            PaymentResponse paymentResponse = new PaymentResponse();
            if (req == null)
            {
                log.LogError("no data to send.");
                paymentResponse.flag = false;
                paymentResponse.message = "You have sent empty content.";
                return (ActionResult)new OkObjectResult(paymentResponse);
            }

            try
            {
                var fileName = $"{Guid.NewGuid().ToString()}.json";
                await paymentSubmitContainer.CreateIfNotExistsAsync();
                CloudBlockBlob blob = paymentSubmitContainer.GetBlockBlobReference(fileName);

                //Send to te container
                PaymentRequest paymentContent = await JsonSerializer.DeserializeAsync<PaymentRequest>(req.Body);
                await blob.UploadTextAsync(JsonSerializer.Serialize(paymentContent));
                log.LogInformation($"Payment sent to Blob {paymentContent.userName}, orderId {paymentContent.orderId}");
                //Sent ot Queue
                await paymentSubmitQueue.AddMessageAsync(new CloudQueueMessage(fileName));
                log.LogInformation($"Payment sent to Queue {paymentContent.userName}, orderId {paymentContent.orderId}");
                paymentResponse.flag = true;
                paymentResponse.message = $"Payment has been added to Queue.";

            }
            catch (Exception ex)
            {
                log.LogError($"error msg : {ex.ToString()}");
                paymentResponse.flag = false;
                paymentResponse.message = ex.ToString();
                return new BadRequestObjectResult(paymentResponse);
            }
            return (ActionResult)new OkObjectResult(paymentResponse);
        }
    }
}
