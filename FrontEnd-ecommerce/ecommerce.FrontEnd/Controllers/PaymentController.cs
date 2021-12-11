using System;
using System.Net.Http;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.RazorHtmlEmails.Services;
using Libraries.ecommerce.RazorHtmlEmails.Views.Emails.Order;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtilityFunctions.Data;
using System.Text.Json;
using ecommerce.FrontEnd.Models;
using Libraries.ecommerce.GoogleReCaptcha.Services;
using Libraries.ecommerce.GoogleReCaptcha.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IService<OrderDetails> serviceOrderDetails;
        private readonly IService<Order> serviceOrder;
        private readonly string urlOrder = "/api/Order/";

        private readonly IService<Address> serviceAddress;
        private readonly string urlAddress = "/api/Address/";

        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";

        private readonly PaymentService servicePayment;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;

        private readonly IService<CheckOut> serviceCheckOut;
        private readonly string urlCart = "/api/Cart/";
        //Google Recaptcha
        private readonly IGoogleReCaptchaService googleReCaptchaService;

        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public PaymentController(IService<Address> _serviceAddress,
            IService<Order> _serviceOrder, PaymentService _servicePayment,
            IService<Setting> _serviceSetting,
            IService<OrderDetails> _serviceOrderDetails,
            IGoogleReCaptchaService _googleReCaptchaService,
            IRazorViewToStringRenderer _razorViewToStringRenderer, IService<CheckOut> _serviceCheckOut,
            ILogger<PaymentController> logger)
        {
            serviceAddress = _serviceAddress;
            serviceOrder = _serviceOrder;
            servicePayment = _servicePayment;
            serviceSetting = _serviceSetting;
            serviceOrderDetails = _serviceOrderDetails;
            razorViewToStringRenderer = _razorViewToStringRenderer;
            googleReCaptchaService = _googleReCaptchaService;
            serviceCheckOut = _serviceCheckOut;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Pay(CheckOut checkOutForm)
        {
            //Check same user Id in back too.
            if (!GetCurrentUserId().Equals(checkOutForm.userId))
            {
                _logger.LogError($"Not Secured Ops, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            bool verified = await googleReCaptchaService.GoogleReCaptchaVerification(new GoogleReCaptchaRequest() { response = checkOutForm.token });
            if (!verified)
            {
                ModelState.AddModelError(string.Empty, "Your Connection is not secure.");
                return View("Checkout", checkOutForm);
            }

            POJO model = new POJO();
            try
            {
                CheckOut checkOutAPI = new CheckOut();
                //Get the website setting.
                var settings = await serviceSetting.GetAll(urlSetting, await GetToken());
                var setting = settings?.FirstOrDefault();

                if (String.IsNullOrEmpty(checkOutForm.orderId))
                {
                    //For more security get the data from the API by user ID the current cart.
                    checkOutAPI = await serviceCheckOut.Get(GetCurrentUserId(), $"{urlCart}CheckOut/", await GetToken());
                    model = await ProcessOrder(checkOutForm);
                }
                else
                {
                    //Get checkOutAPI by orderId
                    checkOutAPI = await serviceCheckOut.Get(checkOutForm.orderId, $"{urlOrder}CheckOutOrder/", await GetToken());
                    if (!checkOutAPI.paymentMethod.Equals(checkOutForm.paymentMethod))
                    {
                        //Update the order payment method (Use change from cash to card <->
                        await serviceOrder.Post(new Order { id = checkOutForm.orderId, modifiedBy = GetCurrentUserId(), paymentMethod = checkOutForm.paymentMethod }, $"{urlOrder}UpdateOrder/", await GetToken());
                    }
                    model.id = checkOutForm.orderId;
                }
                var userAddress = await serviceAddress.Get(checkOutForm.addressId.ToString(), urlAddress, await GetToken());

                if (checkOutForm.paymentMethod.Equals(Data.PaymentMethod.Card))
                {
                    HttpClient httpClient = new HttpClient();
                    var paymentsRedirect = System.Diagnostics.Debugger.IsAttached ? "https://localhost:44322" : $"https://{Request.Host.Value}";
                    PayTabsResponse payTabsResponse = null;


                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(setting.payTabsServerKey);
                    var payTabsRequestContent = JsonSerializer.Serialize(new PaytabsRequest
                    {
                        PofileId = setting.payTabsMerchantProfileID,
                        TransType = "sale",
                        TransClass = "ecom",
                        CartId = model.id,
                        CartDescription = $"Example Product {model.id}",
                        CartCurrency = "USD",
                        CartAmount = checkOutAPI.total,
                        Callback = $"{paymentsRedirect}/Payment/PaymentsRedirect",
                        Return = $"{paymentsRedirect}/Payment/PaymentsRedirect",
                        HideShipping = true,
                        CustomerDetails = new Models.CustomerDetails
                        {
                            Name = checkOutAPI.fullName,
                            Email = checkOutAPI.email,
                            Phone = checkOutAPI.phone,
                            City = userAddress.city,
                            State = userAddress.city,
                            Country = userAddress.country,
                            Street = userAddress.country,
                            IP = Request.HttpContext.Connection.RemoteIpAddress?.ToString()
                        }
                    });
                    HttpContent httpContent = new StringContent(payTabsRequestContent, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(setting.payTabsAPIUrl, httpContent);
                    response.EnsureSuccessStatusCode();
                    var payTabsResponseContent = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(payTabsResponseContent))
                    {
                        payTabsResponse = JsonSerializer.Deserialize<PayTabsResponse>(payTabsResponseContent);
                        checkOutForm.paymentReference = payTabsResponse.TransRef;
                        //We have moved the email sender to be after the payment if suc, than send email.
                        string body = await ProcessEmail(userAddress, checkOutAPI, model);
                        await ProcessPayment(checkOutForm, checkOutAPI, model, body);
                        return Redirect(payTabsResponse.RedirectUrl);
                    }
                    _logger.LogError($"PayTabs issue, Empty Content {payTabsResponseContent} Model Error: {model.message}, UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                else
                {
                    string body = await ProcessEmail(userAddress, checkOutAPI, model);
                    await ProcessPayment(checkOutForm, checkOutAPI, model, body);
                    //We have moved the email sender to be after the payment if suc, than send email.                  
                    return RedirectToAction("OrderConfirmation", "Payment", new { id = model.id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PayTabs issue, Model Error: {model.message}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost, AllowAnonymous]
        public IActionResult PaymentsRedirect(Models.PayTabsTransactionResponse payTabsTransactionResponse)
        {
            try
            {
                return RedirectToAction("PaymentConfirmation", "Payment", payTabsTransactionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PaymentTrans:{payTabsTransactionResponse?.cartId.ToString()}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult PaymentConfirmation(Models.PayTabsTransactionResponse payTabsTransactionResponse)
        {
            try
            {
                return View(payTabsTransactionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PaymentTrans:{payTabsTransactionResponse?.cartId.ToString()}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task<POJO> ProcessOrder(CheckOut checkOut)
        {
            Order order = new Order()
            {
                createDate = DateTime.Now,
                createdBy = GetCurrentUserId(),
                isDeleted = false,
                isPaid = false,
                cartId = checkOut.cartId,
                trackingStatus = TrackingStatus.Ordered,
                paymentMethod = checkOut.paymentMethod
            };
            //In the API side will fill the rest of data (OrderLines) by the cartId
            POJO model = await serviceOrder.Post(order, urlOrder, await GetToken());
            return model;
        }

        private async Task<string> ProcessEmail(Address address, CheckOut checkOutAPI, POJO model)
        {
            //Get the orderlines  (YourOrder)
            OrderDetails orderDetails = await serviceOrderDetails.Get(model.id, $"{urlOrder}YourOrder/", await GetToken());
            //Send email
            OrderEmailViewModel orderEmailViewModel = new OrderEmailViewModel(checkOutAPI.fullName, model.id,
                checkOutAPI.total.ToString(), orderDetails, address, checkOutAPI.subTotal.ToString(),
                checkOutAPI.shippingCost.ToString(), String.Format("{0:0.00}", checkOutAPI.tax), String.Format("{0:0.00}", checkOutAPI.taxedCost));
            string body = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Order/OrderEmail.cshtml", orderEmailViewModel);
            return body;
        }

        private async Task ProcessPayment(CheckOut checkOutForm, CheckOut checkOutAPI, POJO model, string body = null)
        {
            //We have to Move the   UtilityFunctions.Data to ecommerce data.
            UtilityFunctions.Data.PaymentMethod paymentMethod = UtilityFunctions.Data.PaymentMethod.Cash;
            switch (checkOutForm.paymentMethod)
            {
                case Data.PaymentMethod.Card:
                    paymentMethod = UtilityFunctions.Data.PaymentMethod.Card;
                    break;
                case Data.PaymentMethod.Cash:
                    paymentMethod = UtilityFunctions.Data.PaymentMethod.Cash;
                    break;
                default:
                    paymentMethod = UtilityFunctions.Data.PaymentMethod.Cash;
                    break;
            }

            _ = await servicePayment.Pay(new PaymentRequest()
            {
                addressId = checkOutForm.addressId.ToString(),
                email = checkOutAPI.email,
                //OrderId from last opeartion POJO
                orderId = model.id,
                //Convert Payment Mthod from Data to Func.Data  
                paymentMethod = paymentMethod,
                phone = checkOutAPI.phone,
                transactionResponseCode = "Pending",
                userId = GetCurrentUserId(),
                userName = checkOutAPI.fullName,
                paymentReference = checkOutForm.paymentReference,
                emailBody = body
            });
        }

        public IActionResult OrderConfirmation(string id)
        {
            return View(new POJO()
            {
                message = id
            });
        }

        public string GetCurrentUserId()
        {
            return User?.FindFirst(c => c.Type == "sub")?.Value;
        }
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}