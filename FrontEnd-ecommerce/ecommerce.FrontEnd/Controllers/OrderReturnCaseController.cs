using System;
using System.IO;
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
    public class OrderReturnCaseController : Controller
    {
        private readonly ILogger<OrderReturnCaseController> _logger;
        //Calling  services
        private readonly IService<OrderReturnCase> service;
        private readonly string url = "/api/OrderReturnCase/";
        private readonly IService<OrderReturnReason> serviceOrderReturnReason;
        private readonly string urlOrderReturnReason = "/api/OrderReturnReason/";
        private readonly EmailSenderService serviceEmailSender;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";
        //FileUploader
        private readonly FileUploadService fileUploadService;

        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public OrderReturnCaseController(IService<OrderReturnCase> _service, IService<OrderReturnReason> _serviceOrderReturnReason,
                                         EmailSenderService _serviceEmailSender, IRazorViewToStringRenderer _razorViewToStringRenderer,
                                         IService<Setting> _serviceSetting, FileUploadService _FileUploadService,
                                         ILogger<OrderReturnCaseController> logger)
        {
            service = _service;
            serviceOrderReturnReason = _serviceOrderReturnReason;
            serviceEmailSender = _serviceEmailSender;
            razorViewToStringRenderer = _razorViewToStringRenderer;
            serviceSetting = _serviceSetting;
            fileUploadService = _FileUploadService;
            _logger = logger;
        }

        public async Task<IActionResult> OpenOrderReturnCase(string orderId)
        {
            try
            {
                if (String.IsNullOrEmpty(orderId))
                {
                    _logger.LogError($"Empty OrderId, UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                } 
                
                MVOrderReturnCase mVOrderReturnCase = new MVOrderReturnCase()
                {
                    id = 0,
                    orderId = orderId,
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
                        reasonId = mVOrderReturnCase.reasonId,
                        customerNote = mVOrderReturnCase.customerNote,
                        orderId = mVOrderReturnCase.orderId,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        status = Status.Open,
                        email = User?.FindFirst(c => c.Type == "email")?.Value,
                        createdBy = GetCurrentUserId(),
                        modifiedBy = GetCurrentUserId()
                    };

                    model = await service.Post(orderReturnCase, url, await GetToken());
                    //Upload the files
                    if (mVOrderReturnCase.fileToUpload != null)
                    {
                        await FileUploder(mVOrderReturnCase, model);
                    }
                    StatusMessage = $"order Return Case Id {model.id} has been Sent.";
                     
                    //Send Email confiramtion
                    await SendEmail("Thank you, We have received your request.", User?.FindFirst(c => c.Type == "email")?.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Model Error: {model.message}, UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                return RedirectToAction("GetOrder", "Order" , new {Id = mVOrderReturnCase.orderId });
            }
            ModelState.AddModelError("", model.message);
            return View(mVOrderReturnCase);
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

        private async Task FileUploder(MVOrderReturnCase mVOrderReturnCase, POJO model)
        {
           var formFile = mVOrderReturnCase.fileToUpload;            
                if (formFile.Length > 0 && formFile.ContentType.StartsWith("image/"))
                {
                    FileUploaderRequest fileUploaderRequest = new FileUploaderRequest();
                    fileUploaderRequest.contentType = formFile.ContentType;
                    fileUploaderRequest.fileExtention = Path.GetExtension(formFile.FileName);
                    fileUploaderRequest.userId = GetCurrentUserId();
                    fileUploaderRequest.id = Convert.ToInt32(model.id);
                    fileUploaderRequest.sourceController = "OrderReturnCase";

                    using (var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);
                        fileUploaderRequest.content = Convert.ToBase64String(memoryStream.ToArray());
                    } 
                    await fileUploadService.FileUpload(fileUploaderRequest);
                }
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