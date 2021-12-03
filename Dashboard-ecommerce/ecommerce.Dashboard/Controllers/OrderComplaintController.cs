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
    public class OrderComplaintController : Controller
    {
        private readonly IService<OrderComplaint> service;
        private readonly ILogger<OrderComplaintController> _logger;
        private readonly string url = "/api/OrderComplaint/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public OrderComplaintController(IService<OrderComplaint> _service, ILogger<OrderComplaintController> logger)
        {
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<OrderComplaint> orderComplaint = await service.GetAll(url, await GetToken());
                return View(orderComplaint);
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
            return View(new MVOrderComplaint { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MVOrderComplaint mVOrderComplaint)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    OrderComplaint orderComplaint = new OrderComplaint()
                    {
                        id = mVOrderComplaint.id,
                        reason = mVOrderComplaint.reason,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        createdBy = mVOrderComplaint.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId()
                    };

                    model = await service.Post(orderComplaint, url, await GetToken());
                    StatusMessage = $"Order Complaint Id {model.id} has been Added/Updated.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                return RedirectToAction("index");
            }
            ModelState.AddModelError("", model.message);
            return View(mVOrderComplaint);
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
                StatusMessage = $"Order Complaint Id { id} has been deleted.";
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
                OrderComplaint orderComplaint = await service.Get(id, url, await GetToken());
                MVOrderComplaint mVOrderComplaint = new MVOrderComplaint()
                {
                    id = orderComplaint.id,
                    reason = orderComplaint.reason,
                };
                return View("Save", mVOrderComplaint);
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
                OrderComplaint orderComplaint = await service.Get(id, url, await GetToken());
                return View(orderComplaint);
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
