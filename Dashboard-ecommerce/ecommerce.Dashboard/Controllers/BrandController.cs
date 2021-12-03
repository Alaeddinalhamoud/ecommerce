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
using UtilityFunctions.Data;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles ="Admin")]
    public class BrandController : Controller
    {
        private readonly ILogger<BrandController> _logger;
        private readonly IService<Brand> service;
        private readonly string url = "/api/brand/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public BrandController(IService<Brand> _service, ILogger<BrandController> logger)
        { 
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            { 
                IQueryable<Brand> Brands =await service.GetAll(url, await GetToken());
                return View(Brands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> GetDeletedBrands()
        {
            try
            {
                IQueryable<Brand> Brands = await service.GetAll($"{url}GetDeletedBrands", await GetToken());
                return View("Index", Brands);
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
            return View(new MvBrand { id = 0});
        }

        [HttpPost]
        public async Task<IActionResult> Save(MvBrand mVCbrand)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                { 
                    Brand brand = new Brand()
                    {
                        id = mVCbrand.id,
                        name = mVCbrand.name,
                        createdBy = mVCbrand.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId(),
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now
                    };
                    if (mVCbrand.id == 0)
                    {
                        brand.imagePath = "/img/NoImage.jpg";
                    }
                    model = await service.Post(brand, url, await GetToken());
                    if (mVCbrand.updateLogo && model.flag)
                    {
                        return RedirectToAction("Upload", "Media", new FileUploaderRequest() { id = Convert.ToInt32(model.id), sourceController = "Brand" });
                    }
                    if (!mVCbrand.updateLogo && model.flag)
                    {
                        StatusMessage = $"Brand Id {model.id} has been Added.";
                        return RedirectToAction("Details", new { id = model.id });
                    }
                    if (!model.flag)
                    {
                        StatusMessage = $"{model.message.ToString()}";
                        ModelState.AddModelError("", model.message);
                        return View(mVCbrand);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                ModelState.AddModelError("", model.message);
            }
            return View(mVCbrand);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, bool value = false)
        {
            if (id == 0)
            {
                return RedirectToAction("Error", "Home");
            }
            Brand brand = new Brand()
            {
                id = id,
                isDeleted = value,
                updateDate = DateTime.Now,
                modifiedBy = GetCurrentUserId()
            };

            try
            { 
                POJO model = await service.Post(brand, $"{url}UpdateIsDeleted", await GetToken());
                var status = value.Equals(true) ? "Deleted" : "Restored";
                StatusMessage = $"Brand Id {id} has been {status}.";
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
            if(String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error", "Home");
            }

            try
            { 
                Brand brand = await service.Get(id, url, await GetToken());
                MvBrand mVCbrand = new MvBrand()
                {
                    id = brand.id,
                    name = brand.name                   
                };
                return View("Save", mVCbrand);
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
            if(String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error", "Home");
            }

            try
            { 
                Brand brand = await service.Get(id, url, await GetToken());
                return View(brand);
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