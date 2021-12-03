using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
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
    public class VendorApplicationController : Controller
    {
        private readonly IService<VendorApplication> service;
        private readonly IService<VendorApplicationDetails> serviceVendorApplicationDetails;
        private readonly string url = "/api/VendorApplication/";
        //setings
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";
        //Email Services
        private readonly EmailSenderService emailSenderService;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
        private readonly ILogger<VendorApplicationController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public VendorApplicationController(IService<VendorApplication> _service, EmailSenderService _emailSenderService,
            IService<Setting> _serviceSetting, IRazorViewToStringRenderer _razorViewToStringRenderer,
            IService<VendorApplicationDetails> _serviceVendorApplicationDetails, ILogger<VendorApplicationController> logger)
        {
            service = _service;
            emailSenderService = _emailSenderService;
            serviceSetting = _serviceSetting;
            razorViewToStringRenderer = _razorViewToStringRenderer;
            serviceVendorApplicationDetails = _serviceVendorApplicationDetails;
            _logger = logger;
        }

        //The API will send the approved only
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<VendorApplication> vendorApplication = await service.GetAll($"{url}GetApprovedFormApplication", await GetToken());
                //Just show the approved
                return View(vendorApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }
        //The api will send aprove = false
        public async Task<IActionResult> PendingVendorApplication()
        {
            try
            {
                IQueryable<VendorApplication> vendorApplication = await service.GetAll($"{url}GetPendingFormApplication", await GetToken());
                //Just show the Pending
                return View(vendorApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> ApproveVendorApplication(int id, string email)
        {
            if (id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            POJO model = new POJO();

            try
            {
                //1 - Mark as approve
                var vendorApplication = new VendorApplication()
                {
                    id = id,
                    status = Status.Closed,//Means aproved
                    updateDate = DateTime.Now,
                    modifiedBy = GetCurrentUserId()
                };
                model = await service.Post(vendorApplication, $"{url}ApproveVendorApplication", await GetToken());
                StatusMessage = $"Vendor Application Id {id} has been aproved.";
                await SendEmail("Thanks, Your request to change your current account to Seller account has been approved.", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Index", "VendorApplication");
        }

        [HttpPost]
        public async Task<IActionResult> RejectVendorApplication(VendorApplication vendorApplicationData)
        {
            if (vendorApplicationData.id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            POJO model = new POJO();

            try
            {
                //1 - Mark as approve
                var vendorApplication = new VendorApplication()
                {
                    id = vendorApplicationData.id,
                    status = Status.Pending,//Means aproved
                    updateDate = DateTime.Now,
                    modifiedBy = GetCurrentUserId(),
                    note = vendorApplicationData.note
                };
                model = await service.Post(vendorApplication, $"{url}", await GetToken());
                StatusMessage = $"Vendor Application Id {vendorApplicationData.id} has been Rejected.";
                await SendEmail(vendorApplication.note, vendorApplicationData.workEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Index", "VendorApplication");
        }

        [HttpGet]
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
                StatusMessage = $"Application Id {id} has been deleted."; 
                return RedirectToAction("index");
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

            await emailSenderService.SendEmail(
                           new EmailSenderRequest()
                           {
                               senderName = setting.websiteName,
                               from = setting.email,
                               subject = $"Seller Application",
                               to = email,
                               plainTextContent = emailBody,
                               htmlContent = emailBody
                           });
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
                VendorApplicationDetails vendorApplicationDetails = await serviceVendorApplicationDetails.Get(id, $"{url}GetVendorApplicationDetails/", await GetToken());
                return View(vendorApplicationDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
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
