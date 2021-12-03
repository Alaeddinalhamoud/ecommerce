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
    public class OrderReturnCaseController : Controller
    {
        private readonly IService<OrderReturnCase> service;
        private readonly string url = "/api/OrderReturnCase/";

        private readonly IService<OrderReturnReason> serviceOrderReturnReason;
        private readonly string urlOrderReturnReason = "/api/OrderReturnReason/";

        private readonly EmailSenderService serviceEmailSender;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";
        private readonly IService<Order> serviceOrder;
        private readonly string urlOrder = "/api/Order/";
        private readonly IService<PaymentTransaction> servicePaymentTransaction;
        private readonly string urlPaymentTransaction = "/api/PaymentTransaction/";
        private readonly ILogger<OrderReturnCaseController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public OrderReturnCaseController(IService<OrderReturnCase> _service, IService<OrderReturnReason> _serviceOrderReturnReason,
                                         EmailSenderService _serviceEmailSender, IRazorViewToStringRenderer _razorViewToStringRenderer,
                                         IService<Setting> _serviceSetting, IService<Order> _serviceOrder, IService<PaymentTransaction> _servicePaymentTransaction,
                                         ILogger<OrderReturnCaseController> logger)
        {
            service = _service;
            serviceOrderReturnReason = _serviceOrderReturnReason;
            serviceEmailSender = _serviceEmailSender;
            razorViewToStringRenderer = _razorViewToStringRenderer;
            serviceSetting = _serviceSetting;
            serviceOrder = _serviceOrder;
            servicePaymentTransaction = _servicePaymentTransaction;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<OrderReturnCase> orderReturnCase = await service.GetAll(url, await GetToken());
                return View(orderReturnCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        } 

        [HttpPost]
        public async Task<IActionResult> Save(MVOrderReturnCase mVOrderReturnCase)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    OrderReturnCase orderReturnCase = new OrderReturnCase()
                    {
                        id = mVOrderReturnCase.id,
                        updateDate = DateTime.Now,
                        status = mVOrderReturnCase.status,
                        actionsToSolve = mVOrderReturnCase.actionsToSolve,
                        messageToCustomer = mVOrderReturnCase.messageToCustomer,
                        modifiedBy = GetCurrentUserId(),
                        createdBy = mVOrderReturnCase.id.Equals(0) ? GetCurrentUserId() : null
                    };

                    model = await service.Post(orderReturnCase, url, await GetToken());
                    StatusMessage = $"order Return Case Id {model.id} has been Added/Updated.";
                    //Update order if returend
                    if(mVOrderReturnCase.status == Status.Returned)
                    {
                        //get the order.
                        var orderCase = await serviceOrder.Get(mVOrderReturnCase.orderId, urlOrder, await GetToken());
                        orderCase.status = Status.Returned;
                        orderCase.modifiedBy = GetCurrentUserId();
                        //Update the order
                        await serviceOrder.Post(orderCase, $"{urlOrder}UpdateOrder", await GetToken());
                        //Create (-) Transaction
                        //Get the transaction
                        var paymentTransaction = await servicePaymentTransaction.Get(mVOrderReturnCase.orderId, $"{urlPaymentTransaction}GetPaymentTransactionByOrder/", await GetToken());
                        //create refund transaction
                        paymentTransaction.id = $"{paymentTransaction.id}-Refund";
                        paymentTransaction.amount = -orderCase.total;
                        paymentTransaction.createDate = DateTime.Now;
                        paymentTransaction.status = "Refund";
                        await servicePaymentTransaction.Post(paymentTransaction, urlPaymentTransaction, await GetToken());
                    }
                    if (mVOrderReturnCase.sendEmail)
                    {
                        //Send Email confiramtion
                        await SendEmail(mVOrderReturnCase.messageToCustomer, mVOrderReturnCase.email);
                    } 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                return RedirectToAction("index");
            }
            return View(mVOrderReturnCase);
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
                StatusMessage = $"OrderReturnCase Id {id} has been deleted.";
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
                OrderReturnCase orderReturnCase = await service.Get(id, url, await GetToken());
                MVOrderReturnCase mVOrderReturnCase = new MVOrderReturnCase()
                {
                    id = orderReturnCase.id,
                    status = orderReturnCase.status,
                    actionsToSolve = orderReturnCase.actionsToSolve,
                    messageToCustomer = orderReturnCase.messageToCustomer,
                    reasonId = orderReturnCase.reasonId,
                    orderId = orderReturnCase.orderId,
                    customerNote = orderReturnCase.customerNote,
                    email = orderReturnCase.email,
                    reasons = new SelectList(await serviceOrderReturnReason.GetAll(urlOrderReturnReason, await GetToken()), "id", "reason")
                };
                return View("Save", mVOrderReturnCase);
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
                OrderReturnCase orderReturnCase = await service.Get(id, url, await GetToken());
                MVOrderReturnCase mVOrderReturnCase = new MVOrderReturnCase()
                {
                    actionsToSolve = orderReturnCase.actionsToSolve,
                    customerNote = orderReturnCase.customerNote,
                    email = orderReturnCase.email,
                    status = orderReturnCase.status,
                    orderId = orderReturnCase.orderId,
                    messageToCustomer = orderReturnCase.messageToCustomer,
                    reasonId = orderReturnCase.reasonId,
                    id = orderReturnCase.id,
                    imageUrl = orderReturnCase.imageUrl,
                    reasons = new SelectList(await serviceOrderReturnReason.GetAll(urlOrderReturnReason, await GetToken()), "id", "reason")
                };
                return View(mVOrderReturnCase);
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
                               subject = $"Order Return-Case",
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
