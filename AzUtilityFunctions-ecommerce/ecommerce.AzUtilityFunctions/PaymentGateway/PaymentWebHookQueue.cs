using ecommerce.AzUtilityFunctions.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.PaymentGateway
{
    public class PaymentWebHookQueue
    {
        public PaymentWebHookQueue()
        {
        }

        [FunctionName("PaymentWebHookQueue")]
        public async Task Run(
            [QueueTrigger("payments-webhook", Connection = "AzureWebJobsStorage")] string myPaymentItem,
            [Blob("payments-webhook", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentSubmitContainer,
            [Blob("payments-webhook-archive", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentArchiveContainer,
            [Queue("emails-submit", Connection = "AzureWebJobsStorage")] CloudQueue emailSubmitQueue,
            [Blob("emails-submit", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer emailSubmitContainer,
            [Table("paymentWebHookTable", Connection = "AzureWebJobsStorage")] CloudTable paymentCloudTable,
            [Blob("payment-email", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentEmailContainer,
            ILogger log)
        {
            PayTabsTransactionResponse paymentContent = new PayTabsTransactionResponse();
            try
            {
                await emailSubmitContainer.CreateIfNotExistsAsync();
                await emailSubmitQueue.CreateIfNotExistsAsync();
                await paymentSubmitContainer.CreateIfNotExistsAsync();
                await paymentArchiveContainer.CreateIfNotExistsAsync();
                await paymentEmailContainer.CreateIfNotExistsAsync();
                await paymentCloudTable.CreateIfNotExistsAsync();

                var paymentSubmit = paymentSubmitContainer.GetBlockBlobReference(myPaymentItem);
                var paymentArchive = paymentArchiveContainer.GetBlockBlobReference(myPaymentItem);
                paymentContent = JsonSerializer.Deserialize<PayTabsTransactionResponse>(await paymentSubmit.DownloadTextAsync());

                //Add the payment data to CloudTable
                var tableOpertion = TableOperation.InsertOrReplace(new PaymentWebHookMapping()
                {
                    AcquirerMessage = paymentContent.acquirerMessage,
                    CustomerEmail = paymentContent.customerEmail,
                    AcquirerRRN = paymentContent.acquirerRRN,
                    CartId = paymentContent.cartId,
                    TranRef = paymentContent.tranRef,
                    RespCode = paymentContent.respCode,
                    RespStatus = paymentContent.respStatus,
                    Signature = paymentContent.signature,
                    RespMessage = paymentContent.respMessage,
                    PartitionKey = paymentContent.cartId,
                    RowKey = paymentContent.cartId,
                });
                await paymentCloudTable.ExecuteAsync(tableOpertion);
                log.LogInformation($"Order {paymentContent.cartId} has been added to the storage table.");

                await paymentArchive.StartCopyAsync(paymentSubmit);
                await paymentSubmit.DeleteAsync();
                log.LogInformation($"Payment has been sent to Archive, OrderId: {paymentContent.cartId}");

            }
            catch (Exception ex)
            {
                log.LogError($"Error while proccess payment : {myPaymentItem} , {ex.ToString()}");
                //We are sending email in case of exception in the payment
                EmailSenderRequest emailContent = new EmailSenderRequest()
                {
                    to = System.Environment.GetEnvironmentVariable("Tech_Email"),
                    from = System.Environment.GetEnvironmentVariable("SMTP_EAMIL"),
                    subject = "Failure in Payment WebHook",
                    htmlContent = $"Payment WebHook has been not processed, OrderId: {paymentContent.cartId} , error msg : {ex.ToString()}",
                    plainTextContent = $"Payment Webhook has been not processed, OrderId: {paymentContent.cartId} , error msg : {ex.ToString()}",
                    senderName = "Az Services"
                };
                await SendEmail(emailSubmitQueue, emailSubmitContainer, log, emailContent);
            }
        }

        private static async Task SendEmail(CloudQueue emailSubmitQueue, CloudBlobContainer emailSubmitContainer, ILogger log,
            EmailSenderRequest emailContent)
        {
            var fileName = $"{Guid.NewGuid()}.json";
            CloudBlockBlob blob = emailSubmitContainer.GetBlockBlobReference(fileName);

            await blob.UploadTextAsync(JsonSerializer.Serialize(emailContent));
            log.LogInformation($"Email sent to Blob {emailContent.to}");
            //Sent ot Queue
            await emailSubmitQueue.AddMessageAsync(new CloudQueueMessage(fileName));
            log.LogInformation($"Payment Email sent to Queue {emailContent.to}");
        }


    }
}
