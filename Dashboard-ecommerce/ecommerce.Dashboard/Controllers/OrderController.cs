using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    { 
        
        private readonly IService<IEnumerable<Order>> serviceOrder;
        private readonly IService<Order> serviceOrders;
        private readonly IService<OrderDetails> serviceOrderDetails;
        private readonly string url = "/api/Order/";
        private readonly ILogger<OrderController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public OrderController(IService<IEnumerable<Order>> _serviceOrder,
            IService<OrderDetails> _serviceOrderDetails, IService<Order> _serviceOrders, ILogger<OrderController> logger)
        {
            serviceOrder = _serviceOrder;
            serviceOrderDetails = _serviceOrderDetails;
            serviceOrders = _serviceOrders;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PendingOrder()
        {
            try
            {
               IEnumerable<Order> model = await serviceOrder.Get("false", $"{url}GetOrdersByStatus/", await GetToken()); 
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async  Task<IActionResult> PaidOrder()
        {
            try
            {
                IEnumerable<Order> model = await serviceOrder.Get("true", $"{url}GetOrdersByStatus/", await GetToken());
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> DeletedOrder()
        {
            try
            {
                var  model = await serviceOrders.GetAll($"{url}GetDeletedOrders", await GetToken());
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrder(string id)
        {
            OrderDetails model = null;
            try
            {
                model = await serviceOrderDetails.Get(id, $"{url}GetDeletedOrder/", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(model);
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