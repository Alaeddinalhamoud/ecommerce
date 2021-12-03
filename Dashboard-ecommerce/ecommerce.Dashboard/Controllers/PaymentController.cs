using System;
using System.Threading.Tasks;
using ecommerce.Data;
using Libraries.ecommerce.RazorHtmlEmails.Services;
using Libraries.ecommerce.RazorHtmlEmails.Views.Emails.GenericText;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UtilityFunctions.Data;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly PaymentService servicePayment;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public PaymentController( IRazorViewToStringRenderer _razorViewToStringRenderer, PaymentService _servicePayment,
            ILogger<PaymentController> logger)
        {
            razorViewToStringRenderer = _razorViewToStringRenderer;
            servicePayment = _servicePayment;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> pay(PaymentRequest paymentRequest)
        {
            POJO model = new POJO();
            try
            {
                var body = await ProcessEmail($"{paymentRequest.userName}, Thank you. We have received your cash payment.");
                //Get User Email by Id
                //ApplicationUser applicationUser = await serviceUser.Get(paymentRequest.userId, urlUser, await GetToken());
                await ProcessPayment(paymentRequest, body);

                StatusMessage = $"Order {paymentRequest.orderId} has been confirmed.";
                return RedirectToAction("GetOrder", "Order", new { id = paymentRequest.orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task<string> ProcessEmail(string body)
        {
            return await razorViewToStringRenderer.RenderViewToStringAsync(
                "/Views/Emails/GenericText/GenericTextEmail.cshtml",
                new GenericTextEmailViewModel(body));
        }

        private async Task ProcessPayment(PaymentRequest paymentRequest, string body = null)
        {
            //We have to Move the   UtilityFunctions.Data to ecommerce data.
            await servicePayment.Pay(new PaymentRequest()
            {
                addressId = "0", //defult val
                email = paymentRequest.email,
                //OrderId from last opeartion POJO
                orderId = paymentRequest.orderId,
                //Convert Payment Mthod from Data to Func.Data  
                paymentMethod = paymentRequest.paymentMethod,
                phone = paymentRequest.phone,
                transactionResponseCode = "PAID",
                userId = GetCurrentUserId(),
                userName = paymentRequest.userName,
                paymentReference = paymentRequest.paymentReference,
                emailBody = body
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
