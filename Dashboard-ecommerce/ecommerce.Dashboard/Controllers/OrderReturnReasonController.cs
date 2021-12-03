using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Dashboard.Models;
using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderReturnReasonController : Controller
    {
        private readonly IService<OrderReturnReason> service;
        private readonly ILogger<OrderReturnReasonController> _logger;
        private readonly string url = "/api/OrderReturnReason/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public OrderReturnReasonController(IService<OrderReturnReason> _service, ILogger<OrderReturnReasonController> logger)
        {
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<OrderReturnReason> orderReturnReasons = await service.GetAll(url, await GetToken());
                return View(orderReturnReasons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View(new MVOrderReturnReason { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MVOrderReturnReason mVOrderReturnReason)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    OrderReturnReason orderReturnReason = new OrderReturnReason()
                    {
                        id = mVOrderReturnReason.id,
                        reason = mVOrderReturnReason.reason,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        createdBy = mVOrderReturnReason.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId()
                    };

                    model = await service.Post(orderReturnReason, url, await GetToken());
                    StatusMessage = $"Order Return Reason Id {model.id} has been Added/Updated.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                return RedirectToAction("index");
            }
            ModelState.AddModelError("", model.message);
            return View(mVOrderReturnReason);
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
                StatusMessage = $"Order Return Reason Id {id} has been deleted.";
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
                _logger.LogError( $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {
                OrderReturnReason orderReturnReason = await service.Get(id, url, await GetToken());
                MVOrderReturnReason mVOrderReturnReason = new MVOrderReturnReason()
                {
                    id = orderReturnReason.id,
                    reason = orderReturnReason.reason,
                };
                return View("Save", mVOrderReturnReason);
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
                OrderReturnReason orderReturnReason = await service.Get(id, url, await GetToken());
                return View(orderReturnReason);
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
