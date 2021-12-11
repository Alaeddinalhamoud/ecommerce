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
    public static class PaymentWebHook
    {
        [FunctionName("PaymentWebHook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("payments-webhook", Connection = "AzureWebJobsStorage")] CloudQueue paymentWebHookQueue,
            [Blob("payments-webhook", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentWebHookContainer,
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
                await paymentWebHookContainer.CreateIfNotExistsAsync();
                CloudBlockBlob blob = paymentWebHookContainer.GetBlockBlobReference(fileName);

                //Send to te container
                PayTabsTransactionResponse paymentContent = await JsonSerializer.DeserializeAsync<PayTabsTransactionResponse>(req.Body);
                await blob.UploadTextAsync(JsonSerializer.Serialize(paymentContent));
                log.LogInformation($"Payment sent to Blob {paymentContent.customerEmail}, orderId {paymentContent.cartId}");
                //Sent ot Queue
                await paymentWebHookQueue.AddMessageAsync(new CloudQueueMessage(fileName));
                log.LogInformation($"Payment sent to Queue {paymentContent.customerEmail}, orderId {paymentContent.cartId}");
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
