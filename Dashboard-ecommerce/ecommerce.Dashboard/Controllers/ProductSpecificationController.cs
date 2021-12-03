using System;
using System.Collections.Generic; 
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
    public class ProductSpecificationController : Controller
    {
        private readonly IService<ProductSpecification> service;
        private readonly string url = "/api/productspecification/";
        private readonly ILogger<ProductSpecificationController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public ProductSpecificationController(IService<ProductSpecification> _service, ILogger<ProductSpecificationController> logger)
        {
            service = _service;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Save(int id)
        {
            if(id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

           List<MvProductSpecification> productSpecification = new List<MvProductSpecification>()
            {
               new MvProductSpecification{ productId = id }
            };
            MvProductSpecificationList mvProductSpecificationList = new MvProductSpecificationList();
            mvProductSpecificationList.MvProductSpecifications = productSpecification;  
            return View(mvProductSpecificationList);
        }
        //This method will send list of product specifications, better than send more than 1 time.
        //only in one connection.
        [HttpPost]
        public async Task<IActionResult> SaveRange(MvProductSpecificationList mVProductSpecification)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                { 
                    List<ProductSpecification> productSpecifications = new List<ProductSpecification>(); 
                    
                    foreach (var item in mVProductSpecification.MvProductSpecifications)
                    {
                        ProductSpecification productSpecification = new ProductSpecification()
                        {
                            name = item.name,
                            value = item.value,
                            productId = mVProductSpecification.MvProductSpecifications[0].productId,
                            createDate = DateTime.Now,
                            updateDate = DateTime.Now,
                            createdBy = mVProductSpecification.MvProductSpecifications[0].productId.Equals(0) ? GetCurrentUserId() : null,
                            modifiedBy = GetCurrentUserId()
                        };
                        productSpecifications.Add(productSpecification);
                    }
                     model = await service.PostRange(productSpecifications, $"{url}SaveRange/", await GetToken()); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                if (model.flag)
                {
                    StatusMessage = $"Product Specification Id {model.id} has been Added.";
                    return RedirectToAction("Details", "Product", new { id = mVProductSpecification.MvProductSpecifications[0].productId });
                }
                //Need to check it too.
                ModelState.AddModelError("", model.message);
            }
            return View(mVProductSpecification);
        }

        //this action will call UpdateIsDeleted to update IsDeleted
        // the POJO will retrun the product ID
        [HttpPost]
        public async Task<IActionResult> Delete(int productSpecificationId)
        {
            if(productSpecificationId == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            ProductSpecification productSpecification = new ProductSpecification()
            {
                id = productSpecificationId,
                isDeleted = true,
                updateDate = DateTime.Now,
                modifiedBy = GetCurrentUserId()
            };

            try
            { 
                POJO model = await service.Post(productSpecification, $"{url}UpdateIsDeleted", await GetToken());
                StatusMessage = $"Product Specification Id {productSpecificationId} has been Deleted.";
                return RedirectToAction("Details", "Product", new { id = model.id });
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
            if(String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            { 
                ProductSpecification productSpecification = await service.Get(id, url, await GetToken());
                MvProductSpecification mVProductSpecification = new MvProductSpecification()
                {
                    id = productSpecification.id,
                    name = productSpecification.name,
                    value = productSpecification.value,
                    productId = productSpecification.productId
                };
                return View(mVProductSpecification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(MvProductSpecification mVProductSpecification)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                { 
                    ProductSpecification productSpecification = new ProductSpecification()
                    {
                        id = mVProductSpecification.id,
                        name = mVProductSpecification.name,
                        value = mVProductSpecification.value,
                        productId = mVProductSpecification.productId,
                        createdBy = GetCurrentUserId(),
                        modifiedBy = GetCurrentUserId(),
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now
                    };
                    model = await service.Post(productSpecification, url, await GetToken());
                   
                    if (model.flag)
                    {
                        StatusMessage = $"Product Specification Id {model.id} has been Deleted.";
                        return RedirectToAction("Details", "Product", new { id = mVProductSpecification.productId });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                ModelState.AddModelError("", model.message);
            }
            return View(mVProductSpecification);
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