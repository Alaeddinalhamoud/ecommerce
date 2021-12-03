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
    public class ShippingCountryController : Controller
    {
        private readonly IService<ShippingCountry> service;
        private readonly string url = "/api/ShippingCountry/";
        private readonly ILogger<ShippingCountryController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public ShippingCountryController(IService<ShippingCountry> _service, ILogger<ShippingCountryController> logger)
        {
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<ShippingCountry> shippingCountries = await service.GetAll(url, await GetToken());
                return View(shippingCountries);
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
            return View(new MVShippingCountry { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MVShippingCountry mVShippingCountry)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    ShippingCountry shippingCountry = new ShippingCountry()
                    {
                        id = mVShippingCountry.id,
                        country = mVShippingCountry.country,
                        code = mVShippingCountry.code,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        createdBy = mVShippingCountry.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId()
                    };

                    model = await service.Post(shippingCountry, url, await GetToken());
                    StatusMessage = $"Shipping Country Id {model.id} has been Added/Updated.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                return RedirectToAction("index");
            }
            ModelState.AddModelError("", model.message);
            return View(mVShippingCountry);
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
                StatusMessage = $"Shipping Country Id {id} has been deleted.";
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
                ShippingCountry shippingCountry = await service.Get(id, url, await GetToken());
                MVShippingCountry mVShippingCountry = new MVShippingCountry()
                {
                    id = shippingCountry.id,
                    country = shippingCountry.country,
                    code = shippingCountry.code
                };
                return View("Save", mVShippingCountry);
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
                ShippingCountry shippingCountry = await service.Get(id, url, await GetToken());
                return View(shippingCountry);
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

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsNewCountry(string Country)
        {
            try
            {
                var shippingCountry = await service.Get(Country, $"{url}GetByCountry/", await GetToken());
                if (shippingCountry == null)
                {
                    return Json(true);
                }
                else
                {
                    return Json($"The country {Country} is already in use.");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }            
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
