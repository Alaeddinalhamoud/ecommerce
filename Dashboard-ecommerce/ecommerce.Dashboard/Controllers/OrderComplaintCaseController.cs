using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Dashboard.Models;
using ecommerce.Data;
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

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderComplaintCaseController : Controller
    {
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
                                         IService<Setting> _serviceSetting
                                        , ILogger<OrderComplaintCaseController> logger)
        {
            service = _service;
            serviceOrderComplaint = _serviceOrderComplaint;
            serviceEmailSender = _serviceEmailSender;
            razorViewToStringRenderer = _razorViewToStringRenderer;
            serviceSetting = _serviceSetting;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<OrderComplaintCase> orderComplaintCase = await service.GetAll(url, await GetToken());
                return View(orderComplaintCase);
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
                        updateDate = DateTime.Now,
                        status = mVOrderComplaintCase.status,
                        actionsToSolve = mVOrderComplaintCase.actionsToSolve,
                        messageToCustomer = mVOrderComplaintCase.messageToCustomer,
                        createdBy = mVOrderComplaintCase.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId()
                    };

                    model = await service.Post(orderComplaintCase, url, await GetToken());
                    StatusMessage = $"Order Complaint Case Id {model.id} has been Added/Updated.";
                    if (mVOrderComplaintCase.sendEmail)
                    {
                        //Send Email confiramtion
                        await SendEmail(mVOrderComplaintCase.messageToCustomer, mVOrderComplaintCase.email);
                    } 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                return RedirectToAction("index");
            }
            return View(mVOrderComplaintCase);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {
                POJO model = await service.Delete(id, url, await GetToken());
                StatusMessage = $"Order Complaint Case Id {id} has been deleted.";
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {
                OrderComplaintCase orderComplaintCase = await service.Get(id, url, await GetToken());
                MVOrderComplaintCase mVOrderComplaintCase = new MVOrderComplaintCase()
                {
                    id = orderComplaintCase.id,
                    status = orderComplaintCase.status,
                    actionsToSolve = orderComplaintCase.actionsToSolve,
                    messageToCustomer = orderComplaintCase.messageToCustomer,
                    reasonId = orderComplaintCase.reasonId,
                    orderId = orderComplaintCase.orderId,
                    customerNote = orderComplaintCase.customerNote,
                    email = orderComplaintCase.email,
                    reasons = new SelectList(await serviceOrderComplaint.GetAll(urlOrderComplaint, await GetToken()), "id", "reason")
                };
                return View("Save", mVOrderComplaintCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {
                OrderComplaintCase orderComplaintCase = await service.Get(id, url, await GetToken());
                MVOrderComplaintCase mVOrderComplaintCase = new MVOrderComplaintCase()
                {
                    actionsToSolve = orderComplaintCase.actionsToSolve,
                    customerNote = orderComplaintCase.customerNote,
                    email = orderComplaintCase.email,
                    status = orderComplaintCase.status,
                    orderId = orderComplaintCase.orderId,
                    messageToCustomer = orderComplaintCase.messageToCustomer,
                    reasonId = orderComplaintCase.reasonId,
                    id = orderComplaintCase.id,
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
