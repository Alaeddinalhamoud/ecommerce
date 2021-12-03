using System;
using System.Linq;
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
    public class MetaTagController : Controller
    {
        private readonly IService<MetaTag> service;
        private readonly string url = "/api/metatag/";
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";
        private readonly ILogger<MetaTagController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public MetaTagController(IService<MetaTag> _service, IService<Setting> _serviceSetting, ILogger<MetaTagController> logger)
        {
            service = _service;
            serviceSetting = _serviceSetting;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<MetaTag> metas = await service.GetAll(url, await GetToken());
                return View(metas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> MetaTag()
        {
            try
            {
                // 0 is to give the website only
                Setting setting = await serviceSetting.Get("0", urlSetting, await GetToken());
                MetaTag model = await service.Get("0", url, await GetToken());
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    MetaTag metaTag = new MetaTag();
                    metaTag.id = 0;
                    metaTag.image = setting.logo;
                    metaTag.imageAlt = $"{setting.websiteName} Logo";
                    metaTag.metaTagType = MetaTagType.Site;
                    metaTag.sitename = setting.websiteName;
                    return View(metaTag);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> NewMetaTag(int productId, string video, string image, string name)
        {
            try
            {
                if (productId == 0)
                {
                    _logger.LogError($"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

                var settings = await serviceSetting.GetAll(urlSetting, await GetToken());
                var setting = settings?.FirstOrDefault();
                MetaTag metatag = new MetaTag()
                {
                    id = 0,
                    productId = productId,
                    metaTagType = MetaTagType.Product,
                    image = image,
                    video = video,
                    title = name,
                    url = $"/Product/Details/{productId}",
                    imageAlt = $"{name} image.",
                    sitename = setting.websiteName
                };

                return View("MetaTag", metatag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(MetaTag mVMetaTag)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    MetaTag metaTag = new MetaTag()
                    {
                        id = mVMetaTag.id,
                        description = mVMetaTag.description,
                        title = mVMetaTag.title,
                        type = mVMetaTag.type,
                        image = mVMetaTag.image,
                        imageAlt = mVMetaTag.imageAlt,
                        url = mVMetaTag.url,
                        locale = mVMetaTag.locale,
                        sitename = mVMetaTag.sitename,
                        video = mVMetaTag.video,
                        keywords = mVMetaTag.keywords,
                        productId = mVMetaTag.productId,
                        metaTagType = mVMetaTag.metaTagType,
                        createDate = DateTime.Now,
                        createdBy = mVMetaTag.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId(),
                        updateDate = DateTime.Now
                    };
                    model = await service.Post(metaTag, url, await GetToken());
                    if (model.flag)
                    {
                        StatusMessage = $"MetaTag has been Added/Updated.";
                        return RedirectToAction("index");
                    } 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                ModelState.AddModelError("", model.message);
            }
            return View(mVMetaTag);
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
                StatusMessage = $"Meta Tag Id {id} has been deleted.";
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
                MetaTag meta = await service.Get(id, url, await GetToken());
                return View("MetaTag", meta);
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