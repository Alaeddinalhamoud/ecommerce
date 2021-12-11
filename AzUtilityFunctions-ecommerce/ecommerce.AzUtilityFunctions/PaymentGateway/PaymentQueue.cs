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
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.PaymentGateway
{
    public class PaymentQueue
    {
        private readonly IService<Data.Address> serviceAddress;
        private readonly string urlAddress = "/api/address/";
        private readonly IService<Data.OrderAddress> serviceOrderAddress;
        private readonly string urlOrderAddress = "/api/Orderaddress/";
        private readonly IService<Data.Order> serviceOrder;
        private readonly string urlOrder = "/api/order/";
        private readonly IService<Data.PaymentTransaction> servicePaymentTransaction;
        private readonly string urlPaymentTransaction = "/api/PaymentTransaction/";
        private readonly IService<Data.TrackingOrder> serviceTrackingOrder;
        private readonly string urlTrackingOrder = "/api/TrackingOrder/";
        private readonly IService<IEnumerable<Data.OrderLine>> serviceOrderLine;
        private readonly string urlOrderLine = "/api/OrderLine/";
        private readonly IService<Data.Product> serviceProduct;
        private readonly string urlProduct = "/api/Product/";
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/setting/";


        public PaymentQueue(IService<Data.Order> _serviceOrder, IService<Data.Address> _serviceAddress,
            IService<Data.PaymentTransaction> _servicePaymentTransaction, IService<Data.TrackingOrder> _serviceTrackingOrder,
            IService<Data.OrderAddress> _serviceOrderAddress, IService<Setting> _serviceSetting,
            IService<IEnumerable<Data.OrderLine>> _serviceOrderLine, IService<Data.Product> _serviceProduct)
        {
            serviceAddress = _serviceAddress;
            serviceOrder = _serviceOrder;
            servicePaymentTransaction = _servicePaymentTransaction;
            serviceTrackingOrder = _serviceTrackingOrder;
            serviceOrderAddress = _serviceOrderAddress;
            serviceOrderLine = _serviceOrderLine;
            serviceProduct = _serviceProduct;
            serviceSetting = _serviceSetting;
        }

        [FunctionName("PaymentQueue")]
        public async Task Run(
            [QueueTrigger("payments-submit", Connection = "AzureWebJobsStorage")] string myPaymentItem,
            [Blob("payments-submit", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentSubmitContainer,
            [Blob("payments-archive", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentArchiveContainer,
            [Queue("emails-submit", Connection = "AzureWebJobsStorage")] CloudQueue emailSubmitQueue,
            [Blob("emails-submit", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer emailSubmitContainer,
            [Table("paymentCloudTable", Connection = "AzureWebJobsStorage")] CloudTable paymentCloudTable,
            [Blob("payment-email", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer paymentEmailContainer,
            ILogger log)
        {
            POJO model = new POJO();
            PaymentRequest paymentContent = new PaymentRequest();
            try
            {
                var token = await serviceAddress.GetAccessToken();
                await emailSubmitContainer.CreateIfNotExistsAsync();
                await emailSubmitQueue.CreateIfNotExistsAsync();
                await paymentSubmitContainer.CreateIfNotExistsAsync();
                await paymentArchiveContainer.CreateIfNotExistsAsync();
                await paymentEmailContainer.CreateIfNotExistsAsync();
                await paymentCloudTable.CreateIfNotExistsAsync();

                var paymentSubmit = paymentSubmitContainer.GetBlockBlobReference(myPaymentItem);
                var paymentArchive = paymentArchiveContainer.GetBlockBlobReference(myPaymentItem);
                paymentContent = JsonSerializer.Deserialize<PaymentRequest>(await paymentSubmit.DownloadTextAsync());

                //Preper the MetaData order By getting the order from db depends on the orderId
                var userOrder = await serviceOrder.Get(paymentContent.orderId, urlOrder, token);
                Address userAddress = new Address();
                //Preper the shipping address By getting the address from db depends on the AddressId
                if (!paymentContent.addressId.Equals("0"))
                {
                    userAddress = await serviceAddress.Get(paymentContent.addressId, urlAddress, token);
                }

                //IF payement method is card process with card
                if (paymentContent.paymentMethod.Equals(UtilityFunctions.Data.PaymentMethod.Card))
                {
                    if (!String.IsNullOrEmpty(paymentContent.paymentReference))
                    {
                        //To store the email body
                        var emailFileName = $"{Guid.NewGuid().ToString()}.json";

                        CloudBlockBlob blob = paymentEmailContainer.GetBlockBlobReference(emailFileName);
                        await blob.UploadTextAsync(paymentContent.emailBody);

                        //Add the payment data to CloudTable

                        var tableOpertion = TableOperation.InsertOrReplace(new PaymentMapping()
                        {
                            addressId = paymentContent.addressId,
                            email = paymentContent.email,
                            emailFileName = emailFileName,
                            orderId = paymentContent.orderId,
                            paymentMethod = paymentContent.paymentMethod,
                            paymentReference = paymentContent.paymentReference,
                            phone = paymentContent.phone,
                            userId = paymentContent.userId,
                            userName = paymentContent.userName,
                            transactionResponseCode = paymentContent.transactionResponseCode,
                            PartitionKey = paymentContent.orderId,
                            RowKey = paymentContent.orderId,
                            executionCount = 0
                        });
                        await paymentCloudTable.ExecuteAsync(tableOpertion);
                        log.LogInformation($"Order {paymentContent.orderId} has been added to the storage table.");
                    }
                }

                if (paymentContent.paymentMethod.Equals(UtilityFunctions.Data.PaymentMethod.Cash) && paymentContent.transactionResponseCode.Equals("PAID"))
                {
                    //Close Order
                    model = await CloseOrder(paymentContent, userOrder, token);
                    //add transaction Card.                              
                    model = await AddTransaction(paymentContent, userOrder, paymentContent.paymentReference, Guid.NewGuid().ToString(), Data.PaymentMethod.Cash, token);
                    //Reduce the product
                    model = await ReduceProductQty(userOrder.id, paymentContent, token);
                }
                else
                {
                    //Add the address to OrderAddress
                    model = await AddOrderAddress(paymentContent, userAddress, token);
                    // Add Tracking Order Shippment record.
                    model = await AddTrackingOrderRecord(paymentContent, userOrder, token);
                    //Send Cash Email
                    if (paymentContent.paymentMethod.Equals(UtilityFunctions.Data.PaymentMethod.Cash))
                    {
                        var settings = await serviceSetting.GetAll(urlSetting, token);
                        var setting = settings?.FirstOrDefault();
                        EmailSenderRequest emailContent = new EmailSenderRequest()
                        {
                            to = paymentContent.email,
                            receiverName = paymentContent.userName,
                            from = setting.email,
                            cc = setting.salesEmail,
                            senderName = setting.websiteName,
                            subject = $"Medi Shopping Order#{paymentContent.orderId}",
                            htmlContent = paymentContent.emailBody,
                            plainTextContent = paymentContent.emailBody
                        };
                        await SendEmail(emailSubmitQueue, emailSubmitContainer, log, emailContent);
                    }
                }

                await paymentArchive.StartCopyAsync(paymentSubmit);
                await paymentSubmit.DeleteAsync();
                log.LogInformation($"Payment has been sent to Archive, OrderId: {paymentContent.orderId}");

            }
            catch (Exception ex)
            {
                log.LogError($"Error while proccess payment : {myPaymentItem} , {ex.ToString()}");
                //We are sending email in case of exception in the payment
                EmailSenderRequest emailContent = new EmailSenderRequest()
                {
                    to = System.Environment.GetEnvironmentVariable("Tech_Email"),
                    from = System.Environment.GetEnvironmentVariable("SMTP_EAMIL"),
                    subject = "Failure in Payment",
                    htmlContent = $"Payment has been not processed, OrderId: {paymentContent.orderId} , error msg : {ex.ToString()}",
                    plainTextContent = $"Payment has been not processed, OrderId: {paymentContent.orderId} , error msg : {ex.ToString()}",
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

        private async Task<POJO> AddTrackingOrderRecord(PaymentRequest paymentContent, Data.Order userOrder, string token)
        {
            TrackingOrder trackingOrder = new TrackingOrder()
            {
                orderId = userOrder.id,
                trackingStatus = TrackingStatus.Ordered,
                date = DateTime.Now,
                userId = paymentContent.userId,
                courierTrackingNumber = String.Empty,
                curierCopmany = String.Empty,
                trackingUrl = String.Empty,
                email = paymentContent.email
            };
            var model = await serviceTrackingOrder.Post(trackingOrder, urlTrackingOrder, token);
            return model;
        }

        private async Task<POJO> AddOrderAddress(PaymentRequest paymentContent, Data.Address userAddress, string token)
        {
            OrderAddress orderAddress = new OrderAddress()
            {
                orderId = paymentContent.orderId,
                houseNumber = userAddress.houseNumber,
                addressline1 = userAddress.addressline1,
                addressline2 = userAddress.addressline2,
                street = userAddress.street,
                city = userAddress.city,
                code = userAddress.code,
                country = userAddress.country,
                createdBy = userAddress.createdBy,
                longitude = userAddress.longitude,
                latitude = userAddress.latitude,
                createDate = DateTime.Now
            };
            var model = await serviceOrderAddress.Post(orderAddress, urlOrderAddress, token);
            return model;
        }

        private async Task<POJO> CloseOrder(PaymentRequest paymentContent, Order userOrder, string token)
        {
            //Close the payment if oly card(If STC or Cash need to be confirmed) 
            userOrder.isPaid = true;
            userOrder.updateDate = DateTime.Now;
            userOrder.status = Status.Closed;
            userOrder.paymentMethod = paymentContent.paymentMethod.Equals(UtilityFunctions.Data.PaymentMethod.Card) ? Data.PaymentMethod.Card : Data.PaymentMethod.Cash;
            //Convert thr paymentmethod from Fnc to Data ecom
            //userOrder.paymentMethod = paymentMethod; //we have sent it from front with the creation
            userOrder.modifiedBy = paymentContent.userId;
            POJO model = await serviceOrder.Post(userOrder, $"{urlOrder}UpdateOrder", token);
            return model;
        }
        private async Task<POJO> ReduceProductQty(string orderId, PaymentRequest paymentContent, string token)
        {
            POJO model = new POJO();
            //Close the payment if oly card(If STC or Cash need to be confirmed) 
            var orderLines = await serviceOrderLine.Get(orderId, $"{urlOrderLine}GetOrderLines/", token);

            foreach (var item in orderLines)
            {
                var product = await serviceProduct.Get(item.productId.ToString(), urlProduct, token);

                if (product == null)
                    break;

                //reduce the product.
                product.qty = product.qty - item.qty;
                model = await serviceProduct.Post(product, urlProduct, token);
            }
            return model;
        }

        private async Task<POJO> AddTransaction(PaymentRequest paymentContent, Data.Order userOrder, string status, string transId, Data.PaymentMethod paymentMethod, string token)
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
            var paymentModel = await servicePaymentTransaction.Post(paymentTransaction, urlPaymentTransaction, token);
            return paymentModel;
        }
    }
}
