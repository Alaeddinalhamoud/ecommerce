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
    public class AlertController : Controller
    {
        private readonly ILogger<AlertController> _logger;
        private readonly IService<Alert> service;
        private readonly string url = "/api/alert/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public AlertController(IService<Alert> _service, ILogger<AlertController> logger)
        {
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<Alert> alerts = await service.GetAll(url, await GetToken());
                return View(alerts);
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
            return View(new MVAlert { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MVAlert mVAlert)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    Alert alert = new Alert()
                    {
                        id = mVAlert.id,
                        title = mVAlert.title,
                        body = mVAlert.body,
                        isEnabled = mVAlert.isEnabled,
                        alertType = mVAlert.alertType,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        createdBy = mVAlert.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId()
                    };

                    model = await service.Post(alert, url, await GetToken());
                    StatusMessage = $"Alert Id {model.id} has been Added/Updated.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                return RedirectToAction("index");
            }
            ModelState.AddModelError("", model.message);
            return View(mVAlert);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            try
            {
                POJO model = await service.Delete(id, url, await GetToken());
                StatusMessage = $"Alert Id {id} has been deleted.";
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
                return RedirectToAction("Error", "Home");
            }

            try
            {
                Alert alert = await service.Get(id, url, await GetToken());
                MVAlert mVAlert = new MVAlert()
                {
                    id = alert.id,
                    title = alert.title,
                    body = alert.body,
                    isEnabled = alert.isEnabled,
                    alertType = alert.alertType
                };
                return View("Save", mVAlert);
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
                return RedirectToAction("Error", "Home");
            }

            try
            {
                Alert alert = await service.Get(id, url, await GetToken());
                return View(alert);
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
