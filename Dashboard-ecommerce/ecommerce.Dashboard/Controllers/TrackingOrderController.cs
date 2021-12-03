using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Dashboard.Models;
using ecommerce.Data;
using Libraries.ecommerce.RazorHtmlEmails.Services;
using Libraries.ecommerce.RazorHtmlEmails.Views.Emails.TrackingShippment;
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
    public class TrackingOrderController : Controller
    {
        private readonly ILogger<TrackingOrderController> _logger;
        private readonly IService<TrackingOrder> service;
        private readonly string url = "/api/TrackingOrder/";
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";
        //Email Services
        private readonly EmailSenderService emailSenderService;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public TrackingOrderController(IService<TrackingOrder> _service, EmailSenderService _emailSenderService,
            IService<Setting> _serviceSetting, IRazorViewToStringRenderer _razorViewToStringRenderer,
            ILogger<TrackingOrderController> logger)
        {
            service = _service;
            emailSenderService = _emailSenderService;
            serviceSetting = _serviceSetting;
            razorViewToStringRenderer = _razorViewToStringRenderer;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> PendingShipment()
        {
            try
            {
                IQueryable<TrackingOrder> trackingOrders = await service.GetAll($"{url}PendingShipments", await GetToken());
                return View(trackingOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> ShippedOrders()
        {
            try
            {
                IQueryable<TrackingOrder> trackingOrders = await service.GetAll($"{url}ShippedOrders", await GetToken());
                return View(trackingOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(MVTrackingOrder mVTrackingOrder)
        {
            POJO model;
            if (mVTrackingOrder == null)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            try
            {
                // 1- Update the record.
                TrackingOrder trackingOrder = new TrackingOrder()
                {
                    id = mVTrackingOrder.trackingId,
                    courierTrackingNumber = mVTrackingOrder.courierTrackingNumber,
                    curierCopmany = mVTrackingOrder.curierCopmany,
                    date = DateTime.Now,
                    orderId = mVTrackingOrder.orderId,
                    trackingStatus = mVTrackingOrder.trackingStatus,
                    trackingUrl = mVTrackingOrder.trackingUrl,
                    updateDate = DateTime.Now,
                    userId = GetCurrentUserId(),
                    email = mVTrackingOrder.email,
                    expectedArrival = mVTrackingOrder.expectedArrivalDate
                };
                model = await service.Post(trackingOrder, url, await GetToken());

                // 2 - Update the orderStatus
                //API Job in Tracking order Post
                // 3 - Send Email to the user. // Fetch setting
                await SendEmail(mVTrackingOrder, trackingOrder);

                StatusMessage = $"Order#{mVTrackingOrder.orderId} has been {mVTrackingOrder.trackingStatus}.";
                return RedirectToAction("PendingShipment");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task SendEmail(MVTrackingOrder mVTrackingOrder, TrackingOrder trackingOrder)
        {
            var settings = await serviceSetting.GetAll(urlSetting, await GetToken());
            var setting = settings?.FirstOrDefault();
            //Preper Template
            TrackingShippmentEmailViewModel trackingShippmentEmailViewModel =
                new TrackingShippmentEmailViewModel(
                     trackingOrder.orderId, trackingOrder.courierTrackingNumber,
                     trackingOrder.curierCopmany, trackingOrder.trackingStatus,
                     trackingOrder.expectedArrival, trackingOrder.trackingUrl);

            string body = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/TrackingShippment/TrackingShippmentEmail.cshtml", trackingShippmentEmailViewModel);

            await emailSenderService.SendEmail(
                           new EmailSenderRequest()
                           {
                               senderName = setting.websiteName,
                               from = setting.email,
                               subject = $"Order#{mVTrackingOrder.orderId} has been {mVTrackingOrder.trackingStatus}.",
                               to = mVTrackingOrder.email,
                               plainTextContent = body,
                               htmlContent = body
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