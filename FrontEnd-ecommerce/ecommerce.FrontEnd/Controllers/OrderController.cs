using System;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        //Calling  services
        private readonly IService<Order> service;
        private readonly string url = "/api/Order/";
        private readonly IService<OrderDetails> serviceOrderDetails;
        private readonly IService<CheckOut> serviceCheckOut;
        private readonly ILogger<OrderController> _logger;

        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public OrderController(IService<Order> _service,
            IService<OrderDetails> _serviceOrderDetails, IService<CheckOut> _serviceCheckOut, ILogger<OrderController> logger)
        {
            service = _service;
            serviceOrderDetails = _serviceOrderDetails;
            serviceCheckOut = _serviceCheckOut;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetOrder(string id)
        {
            OrderDetails model = null;
            try
            {
                model = await serviceOrderDetails.Get(id, $"{url}YourOrder/", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(model);

        }

        public async Task<IActionResult> Delete(string id, string userId)
        {
            POJO model = null;
            try
            {
                if (!userId.Equals(GetCurrentUserId()))
                {
                    _logger.LogError($"UserId: {GetCurrentUserId()}");
                    throw new Exception($"Deleting Order {id}, by {GetCurrentUserId()}  Not secured.");
                }
                var order = await service.Get(id, url, await GetToken());
                order.isDeleted = true;
                order.modifiedBy = GetCurrentUserId();
                order.status = Status.Deleted;
                order.trackingStatus = TrackingStatus.Deleted;
                model = await service.Post(order, $"{url}DeleteOrder", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Model: {model.message}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("MyOrders", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> CheckoutOrder(string id)
        {
            POJO model = new POJO();
            CheckOut checkOut = new CheckOut();
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    checkOut = await serviceCheckOut.Get(id.ToString(), $"{url}checkOutOrder/", await GetToken());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error model: {model.message}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            //send MV addCheckoutress total orderID
            return View("Checkout", checkOut);
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