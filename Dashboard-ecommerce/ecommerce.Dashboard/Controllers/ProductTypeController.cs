using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using ecommerce.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Libraries.ecommerce.Services.Services;
using Microsoft.Extensions.Logging;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductTypeController : Controller
    {
        private readonly IService<ProductType> service;
        private readonly string url = "/api/producttype/";
        private readonly ILogger<ProductTypeController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public ProductTypeController(IService<ProductType> _service, ILogger<ProductTypeController> logger)
        {
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<ProductType> productType = await service.GetAll(url, await GetToken());
                return View(productType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> GetDeletedProductTypes()
        {
            try
            {
                IQueryable<ProductType> productType = await service.GetAll($"{url}GetDeletedProductTypes", await GetToken());
                return View("Index", productType);
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
            return View(new MvProductType { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MvProductType mVCproductType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ProductType productType = new ProductType()
                    {
                        id = mVCproductType.id,
                        name = mVCproductType.name,
                        createdBy = mVCproductType.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId(),
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now
                    };
                    POJO model = await service.Post(productType, url, await GetToken());

                    if (!model.flag)
                    {
                        StatusMessage = $"{model.message.ToString()}";
                        ModelState.AddModelError("", model.message);
                        return View(mVCproductType);
                    }

                    StatusMessage = $"Product Type Id {model.id} has been Added.";
                    return RedirectToAction("Details", new { id = model.id });  
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                } 
            }
            return View(mVCproductType);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, bool value = false)
        {
            if (id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            ProductType productType = new ProductType()
            {
                id = id,
                isDeleted = value,
                updateDate = DateTime.Now,
                modifiedBy = GetCurrentUserId()
            };

            try
            {
                POJO model = await service.Post(productType, $"{url}UpdateIsDeleted", await GetToken());
                var status = value.Equals(true) ? "Deleted" : "Restored";
                StatusMessage = $"Product type Id {id} has been {status}.";
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
                ProductType productType = await service.Get(id, url, await GetToken());
                MvProductType mVCproductType = new MvProductType()
                {
                    id = productType.id,
                    name = productType.name
                };
                return View("Save", mVCproductType);
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
                ProductType productType = await service.Get(id, url, await GetToken());
                return View(productType);
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