using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.FrontEnd.Models;
using Libraries.ecommerce.RazorHtmlEmails.Services;
using Libraries.ecommerce.RazorHtmlEmails.Views.Emails.GenericText;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using UtilityFunctions.Data;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class OrderComplaintCaseController : Controller
    {
        //Calling  services
        private readonly IService<OrderComplaintCase> service;
        private readonly string url = "/api/OrderComplaintCase/";
        private readonly IService<OrderComplaint> serviceOrderComplaint;
        private readonly string urlOrderComplaint = "/api/OrderComplaint/";
        private readonly EmailSenderService serviceEmailSender;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";
        private readonly ILogger<OrderComplaintCaseController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public OrderComplaintCaseController(IService<OrderComplaintCase> _service, IService<OrderComplaint> _serviceOrderComplaint,
                                         EmailSenderService _serviceEmailSender, IRazorViewToStringRenderer _razorViewToStringRenderer,
                                         IService<Setting> _serviceSetting, ILogger<OrderComplaintCaseController> logger)
        {
            service = _service;
            serviceOrderComplaint = _serviceOrderComplaint;
            serviceEmailSender = _serviceEmailSender;
            razorViewToStringRenderer = _razorViewToStringRenderer;
            serviceSetting = _serviceSetting;
            _logger = logger;
        }

        public async Task<IActionResult> OpenOrderComplaintCase(string orderId)
        {
            try
            {
                if (String.IsNullOrEmpty(orderId))
                {
                    _logger.LogError($"Empty Data, UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                MVOrderComplaintCase mVOrderComplaintCase = new MVOrderComplaintCase()
                {
                    id = 0,
                    orderId = orderId,
                    reasons = new SelectList(await serviceOrderComplaint.GetAll(urlOrderComplaint, await GetToken()), "id", "reason")
                };
                return View(mVOrderComplaintCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(MVOrderComplaintCase mVOrderComplaintCase)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    OrderComplaintCase orderComplaintCase = new OrderComplaintCase()
                    {
                        id = mVOrderComplaintCase.id,
                        reasonId = mVOrderComplaintCase.reasonId,
                        customerNote = mVOrderComplaintCase.customerNote,
                        orderId = mVOrderComplaintCase.orderId,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        status = Status.Open,
                        email = User?.FindFirst(c => c.Type == "email")?.Value,
                        createdBy = GetCurrentUserId(),
                        modifiedBy = GetCurrentUserId()
                    };

                    model = await service.Post(orderComplaintCase, url, await GetToken());
                    StatusMessage = $"Order Complaint Case Id {model.id} has been Sent.";

                    //Send Email confiramtion
                    await SendEmail("Thank you, We have received your request.", User?.FindFirst(c => c.Type == "email")?.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Model error: {model.message}, UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                return RedirectToAction("GetOrder", "Order", new { Id = mVOrderComplaintCase.orderId });
            }
            ModelState.AddModelError("", model.message);
            return View(mVOrderComplaintCase);
        }

        private async Task SendEmail(string body, string email)
        {
            var settings = await serviceSetting.GetAll(urlSetting, await GetToken());
            var setting = settings?.FirstOrDefault();
            string emailBody = await razorViewToStringRenderer.RenderViewToStringAsync(
                "/Views/Emails/GenericText/GenericTextEmail.cshtml",
                new GenericTextEmailViewModel(body));

            await serviceEmailSender.SendEmail(
                           new EmailSenderRequest()
                           {
                               senderName = setting.websiteName,
                               from = setting.email,
                               cc = setting.email,
                               subject = $"Order Complaint-Case",
                               to = email,
                               plainTextContent = emailBody,
                               htmlContent = emailBody
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