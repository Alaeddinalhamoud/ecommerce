using ecommerce.AzUtilityFunctions.Models;
using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.PaymentGateway
{
    public class SyncVerifyPayment
    {
        private readonly IService<Data.Order> serviceOrder;
        private readonly string urlOrder = "/api/order/";
        private readonly IService<IEnumerable<Data.OrderLine>> serviceOrderLine;
        private readonly string urlOrderLine = "/api/OrderLine/";
        private readonly IService<Data.Product> serviceProduct;
        private readonly string urlProduct = "/api/Product/";
        private readonly IService<Data.PaymentTransaction> servicePaymentTransaction;
        private readonly string urlPaymentTransaction = "/api/PaymentTransaction/";
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/setting/";
        private readonly HttpClient httpClient;

        public SyncVerifyPayment(IService<Data.Order> _serviceOrder, HttpClient _httpClient, IService<Data.PaymentTransaction> _servicePaymentTransaction,
                           IService<IEnumerable<Data.OrderLine>> _serviceOrderLine, IService<Data.Product> _serviceProduct, IService<Setting> _serviceSetting)
        {
            serviceOrder = _serviceOrder;
            serviceOrderLine = _serviceOrderLine;
            serviceProduct = _serviceProduct;
            servicePaymentTransaction = _servicePaymentTransaction;
            httpClient = _httpClient;
            serviceSetting = _serviceSetting;
        }
        [FunctionName("SyncVerifyPayment")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
        [Table("paymentCloudTable", Connection = "AzureWebJobsStorage")] CloudTable paymentCloudTable,
        [Blob("payment-email", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentEmailContainer,
        [Queue("emails-submit", Connection = "AzureWebJobsStorage")] CloudQueue emailSubmitQueue,
        [Blob("emails-submit", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer emailSubmitContainer,
        ILogger log)
        {
            log.LogInformation($"Payment Timer trigger function executed at: {DateTime.Now}");
            string orderId = String.Empty;
            try
            {
                var token = await serviceOrder.GetAccessToken();
                await paymentCloudTable.CreateIfNotExistsAsync();
                await paymentEmailContainer.CreateIfNotExistsAsync();
                await emailSubmitQueue.CreateIfNotExistsAsync();
                await emailSubmitContainer.CreateIfNotExistsAsync();

                foreach (PaymentMapping payment in await paymentCloudTable.ExecuteQuerySegmentedAsync(new TableQuery<PaymentMapping>(), null))
                {
                    orderId = payment.orderId;
                    if (payment.executionCount.Equals(5))
                    {
                        await paymentCloudTable.ExecuteAsync(TableOperation.Delete(payment));
                        log.LogError($"Order {payment.orderId} has been deleted");
                        EmailSenderRequest emailContent = new EmailSenderRequest()
                        {
                            to = System.Environment.GetEnvironmentVariable("Tech_Email"),
                            from = System.Environment.GetEnvironmentVariable("SMTP_EAMIL"),
                            subject = "Failure in Payment",
                            htmlContent = $"Payment has been not processed, OrderId: {payment.orderId} ",
                            plainTextContent = $"Payment has been not processed, OrderId: {payment.orderId}",
                            senderName = "Az Services"
                        };
                        await SendEmail(emailSubmitQueue, emailSubmitContainer, log, emailContent);
                        continue;
                    }

                    PayTabsVerifyPaymentResponse payTabsVerifyPaymentResponse = new PayTabsVerifyPaymentResponse();

                    //Call PayTabs
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Environment.GetEnvironmentVariable("PayTabs_PROFILE_SERVER_KEY"));
                    var payTabsRequestContent = JsonSerializer.Serialize(new
                    {
                        profile_id = Convert.ToInt32(Environment.GetEnvironmentVariable("PayTabs_PROFILE_ID")),
                        tran_ref = payment.paymentReference
                    });
                    HttpContent httpContent = new StringContent(payTabsRequestContent, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(Environment.GetEnvironmentVariable("PayTabs_API_URL"), httpContent);

                    if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                    {
                        await paymentCloudTable.ExecuteAsync(TableOperation.Delete(payment));
                        log.LogError($"Order {payment.orderId} has been deleted");
                        continue;
                    }

                    var payTabsResponseContent = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(payTabsResponseContent) && response.IsSuccessStatusCode)
                    {

                        payTabsVerifyPaymentResponse = JsonSerializer.Deserialize<PayTabsVerifyPaymentResponse>(payTabsResponseContent);
                        if (payTabsVerifyPaymentResponse.payment_result.response_status.Equals("A"))
                        {
                            try
                            {
                                var userOrder = await serviceOrder.Get(payment.orderId, urlOrder, token);
                                //Close Order
                                await CloseOrder(payment, userOrder, token);
                                //add transaction Card.                              
                                await AddTransaction(payment, userOrder, payTabsVerifyPaymentResponse?.payment_result.acquirer_message, payTabsVerifyPaymentResponse?.tran_ref, Data.PaymentMethod.Card, token);
                                //Reduce the product
                                await ReduceProductQty(payment.orderId, payment, token);

                                //Send Email
                                var emailBodyContainer = paymentEmailContainer.GetBlockBlobReference(payment.emailFileName);
                                var body = await emailBodyContainer.DownloadTextAsync();
                                //Need to add email sender
                                var settings = await serviceSetting.GetAll(urlSetting, token);
                                var setting = settings?.FirstOrDefault();
                                EmailSenderRequest emailContent = new EmailSenderRequest()
                                {
                                    to = payment.email,
                                    receiverName = payment.userName,
                                    from = setting.email,
                                    cc = setting.salesEmail,
                                    senderName = setting.websiteName,
                                    subject = $"Medi Shopping Order# {payment.orderId}",
                                    htmlContent = body,
                                    plainTextContent = body
                                };
                                await SendEmail(emailSubmitQueue, emailSubmitContainer, log, emailContent);

                                //Delete if payment and email success 
                                await emailBodyContainer.DeleteAsync();

                                log.LogInformation($"payment has been confirmed");
                                await paymentCloudTable.ExecuteAsync(TableOperation.Delete(payment));
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("401"))
                                {
                                    log.LogError($"payment Token has been expired {ex}");
                                    await paymentCloudTable.ExecuteAsync(TableOperation.Delete(payment));
                                }
                                continue;
                            }
                        }
                        else
                        {
                            payment.executionCount += 1;
                            await paymentCloudTable.ExecuteAsync(TableOperation.Replace(payment));
                            log.LogInformation($"Order {payment.orderId} has be incresed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Sync Payment issue. {ex}");
                //We have to send email to the tech team
                EmailSenderRequest emailContent = new EmailSenderRequest()
                {
                    to = Environment.GetEnvironmentVariable("Tech_Email"),
                    from = Environment.GetEnvironmentVariable("SMTP_EAMIL"),
                    subject = "Failure Sync Verify  Payment",
                    htmlContent = $"Payment has been not processed, OrderId: {orderId} , error msg : {ex.ToString()}",
                    plainTextContent = $"Payment has been not processed, OrderId: {orderId} , error msg : {ex.ToString()}",
                    senderName = "Medi Shopping - Az Services"
                };
                await SendEmail(emailSubmitQueue, emailSubmitContainer, log, emailContent);
            }
        }
        private async Task SendEmail(CloudQueue emailSubmitQueue, CloudBlobContainer emailSubmitContainer, ILogger log,
           EmailSenderRequest emailContent)
        {
            await emailSubmitContainer.CreateIfNotExistsAsync();
            var fileName = $"{Guid.NewGuid()}.json";
            CloudBlockBlob blob = emailSubmitContainer.GetBlockBlobReference(fileName);

            await blob.UploadTextAsync(JsonSerializer.Serialize(emailContent));
            log.LogInformation($"Email sent to Blob {emailContent.to}");
            //Sent ot Queue
            await emailSubmitQueue.AddMessageAsync(new CloudQueueMessage(fileName));
            log.LogInformation($"Payment Email sent to Queue {emailContent.to}");
        }
        private async Task CloseOrder(PaymentMapping paymentContent, Order userOrder, string token)
        {
            //Close the payment if oly card(If STC or Cash need to be confirmed) 
            userOrder.isPaid = true;
            userOrder.updateDate = DateTime.Now;
            userOrder.status = Status.Closed;
            //Convert thr paymentmethod from Fnc to Data ecom
            // userOrder.paymentMethod = paymentMethod; //we have sent it from front with the creation
            userOrder.modifiedBy = paymentContent.userId;
            await serviceOrder.Post(userOrder, $"{urlOrder}UpdateOrder", token);
        }
        private async Task ReduceProductQty(string orderId, PaymentMapping paymentContent, string token)
        {
            //Close the payment if oly card(If STC or Cash need to be confirmed) 
            var orderLines = await serviceOrderLine.Get(orderId, $"{urlOrderLine}GetOrderLines/", token);

            foreach (var item in orderLines)
            {
                var product = await serviceProduct.Get(item.productId.ToString(), urlProduct, token);

                if (product == null)
                    break;

                //reduce the product.
                product.qty = product.qty - item.qty;
                await serviceProduct.Post(product, urlProduct, token);
            }
        }

        private async Task AddTransaction(PaymentMapping paymentContent, Data.Order userOrder, string status, string transId, Data.PaymentMethod paymentMethod, string token)
        {
            var paymentTransaction = new PaymentTransaction()
            {
                amount = userOrder.total,
                createDate = DateTime.Now,
                fullName = paymentContent.userName,
                orderId = userOrder.id,
                userId = paymentContent.userId,
                status = status,
                id = transId,
                paymentMethod = paymentMethod
            };
            await servicePaymentTransaction.Post(paymentTransaction, urlPaymentTransaction, token);
        }
    }
}
