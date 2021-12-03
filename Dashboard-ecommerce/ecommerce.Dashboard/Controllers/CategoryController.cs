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
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IService<Category> service;
        private readonly string url = "/api/category/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public CategoryController(IService<Category> _service, ILogger<CategoryController> logger)
        {
            service = _service;
            _logger = logger;
        }
        // [Authorize("Customer")]
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<Category> categories = await service.GetAll(url, await GetToken());
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> GetDeletedCategory()
        {
            try
            {
                IQueryable<Category> categories = await service.GetAll($"{url}GetDeletedCategory", await GetToken());
                return View("Index", categories);
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
            return View(new MvCategory { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MvCategory mVCategory)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    Category category = new Category()
                    {
                        id = mVCategory.id,
                        name = mVCategory.name,
                        createdBy = mVCategory.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId(),
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now
                    };

                    if (mVCategory.id == 0)
                    {
                        category.imagePath = "/img/NoImage.jpg";
                    }
                    model = await service.Post(category, url, await GetToken());

                    if (mVCategory.updatelogo && model.flag)
                    {
                        return RedirectToAction("Upload", "Media", new FileUploaderRequest() { id = Convert.ToInt32(model.id), sourceController = "Category" });
                    }

                    if (model.flag && !mVCategory.updatelogo)
                    {
                        StatusMessage = $"Category Id {model.id} has been Added.";
                        return RedirectToAction("Details", new { id = model.id });
                    }
                    if (!model.flag)
                    {
                        StatusMessage = $"{model.message.ToString()}";
                        ModelState.AddModelError("", model.message);
                        return View(mVCategory);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                ModelState.AddModelError("", model.message);
            }
            return View(mVCategory);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, bool value = false)
        {
            if (id == 0)
            {
                return RedirectToAction("Error", "Home");
            }
            Category category = new Category()
            {
                id = id,
                isDeleted = value,
                updateDate = DateTime.Now,
                modifiedBy = GetCurrentUserId()
            };

            try
            {
                POJO model = await service.Post(category, $"{url}UpdateIsDeleted", await GetToken());
                var status = value.Equals(true) ? "Deleted" : "Restored";
                StatusMessage = $"Category Id {id} has been {status}.";
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
                Category category = await service.Get(id, url, await GetToken());
                MvCategory mVCategory = new MvCategory()
                {
                    id = category.id,
                    name = category.name
                };
                return View("Save", mVCategory);
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
                Category category = await service.Get(id, url, await GetToken());
                return View(category);
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