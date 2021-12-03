using System;
using System.Threading.Tasks;
using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentTransactionController : Controller
    {
        private readonly IService<PaymentTransaction> service;
        private readonly string url = "/api/PaymentTransaction/";
        private readonly ILogger<PaymentTransactionController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public PaymentTransactionController(IService<PaymentTransaction> _service, ILogger<PaymentTransactionController> logger)
        {
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        { 
            try
            {
                var model = await service.GetAll(url, await GetToken());
                return View(model);
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