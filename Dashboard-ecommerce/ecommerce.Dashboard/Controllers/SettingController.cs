using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Dashboard.Models;
using ecommerce.Data;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UtilityFunctions.Data;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly IService<Setting> service;
        private readonly string url = "/api/setting/";
        //FileUploader
        private readonly FileUploadService fileUploadService;
        private readonly ILogger<SettingController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public SettingController(IService<Setting> _service, FileUploadService _FileUploadService, ILogger<SettingController> logger)
        {
            service = _service;
            fileUploadService = _FileUploadService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AdvancedSettings()
        {
            try
            {
                var settings = await service.GetAll(url, await GetToken());
                var setting = settings?.FirstOrDefault();
                if (setting != null)
                {
                    MVSetting mVSetting = new MVSetting()
                    {
                        id = setting.id,
                        email = setting.email,
                        isMostViewdProduct = setting.isMostViewdProduct,
                        isSlider = setting.isSlider,
                        logo = setting.logo,
                        pageSize = setting.pageSize,
                        websiteName = setting.websiteName,
                        numberOfSliders = setting.numberOfSliders,
                        description = setting.description,
                        address = setting.address,
                        phone = setting.phone,
                        isNewArrivalProduct = setting.isNewArrivalProduct,
                        isRecentlyViewedProduct = setting.isRecentlyViewedProduct,
                        tidioScript = setting.tidioScript,
                        isTidioScript = setting.isTidioScript,
                        tax = setting.tax,
                        shippingCost = setting.shippingCost,
                        createDate = DateTime.Now,
                        createdBy = GetCurrentUserId(),
                        modifiedBy = GetCurrentUserId(),
                        updateDate = DateTime.Now,
                        salesEmail = setting.salesEmail,
                        isCard = setting.isCard,
                        isCash = setting.isCash,
                        enableMaintenance = setting.enableMaintenance,
                        isVendorEnabled = setting.isVendorEnabled,
                        helpDeskEmail = setting.helpDeskEmail,
                        orderReturnDays = setting.orderReturnDays,
                        enableCoupon = setting.enableCoupon,
                        payTabsClientKey = setting.payTabsClientKey,
                        payTabsMerchantProfileID = setting.payTabsMerchantProfileID,
                        payTabsServerKey = setting.payTabsServerKey,
                        payTabsAPIUrl = setting.payTabsAPIUrl,
                        facebookLink = setting.facebookLink,
                        twitterLink = setting.twitterLink,
                        instgramLink = setting.instgramLink
                    };
                    return View(mVSetting);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(new MVSetting() { id = 0 });
        }


        [HttpPost]
        public async Task<IActionResult> Save(MVSetting mVSetting)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    Setting setting = new Setting()
                    {
                        id = mVSetting.id,
                        websiteName = mVSetting.websiteName,
                        email = mVSetting.email,
                        isMostViewdProduct = mVSetting.isMostViewdProduct,
                        isSlider = mVSetting.isSlider,
                        numberOfSliders = mVSetting.numberOfSliders,
                        pageSize = mVSetting.pageSize,
                        createdBy = GetCurrentUserId(),
                        modifiedBy = GetCurrentUserId(),
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        address = mVSetting.address,
                        description = mVSetting.description,
                        phone = mVSetting.phone,
                        isNewArrivalProduct = mVSetting.isNewArrivalProduct,
                        isRecentlyViewedProduct = mVSetting.isRecentlyViewedProduct,
                        tidioScript = mVSetting.tidioScript,
                        isTidioScript = mVSetting.isTidioScript,
                        tax = mVSetting.tax,
                        shippingCost = mVSetting.shippingCost,
                        salesEmail = mVSetting.salesEmail,
                        isCard = mVSetting.isCard,
                        isCash = mVSetting.isCash,
                        enableMaintenance = mVSetting.enableMaintenance,
                        isVendorEnabled = mVSetting.isVendorEnabled,
                        helpDeskEmail = mVSetting.helpDeskEmail,
                        orderReturnDays = mVSetting.orderReturnDays,
                        enableCoupon = mVSetting.enableCoupon,
                        payTabsServerKey = mVSetting.payTabsServerKey,
                        payTabsMerchantProfileID = mVSetting.payTabsMerchantProfileID,
                        payTabsClientKey = mVSetting.payTabsClientKey,
                        payTabsAPIUrl = mVSetting.payTabsAPIUrl,
                        facebookLink = mVSetting.facebookLink,
                        instgramLink = mVSetting.instgramLink,
                        twitterLink = mVSetting.twitterLink
                    };

                    if (mVSetting.id == 0)
                    {
                        mVSetting.logo = "/img/NoImage.jpg";
                    }
                    model = await service.Post(setting, url, await GetToken());

                    //Upload the files
                    if (mVSetting.fileToUpload != null)
                    {
                        await FileUploder(mVSetting, model);
                    }

                    if (mVSetting.isLogo && model.flag)
                    {
                        return RedirectToAction("Upload", "Media", new FileUploaderRequest() { id = Convert.ToInt32(model.id), sourceController = "Setting" });
                    }

                    if (model.flag && !mVSetting.isLogo)
                    {
                        StatusMessage = $"Settings has been Updated.";
                        return RedirectToAction("AdvancedSettings");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                ModelState.AddModelError("", model.message);
            }
            return View(mVSetting);
        }

        private async Task FileUploder(MVSetting mVSetting, POJO model)
        {

            if (mVSetting.fileToUpload.Length > 0 && mVSetting.fileToUpload.ContentType.Equals("application/pdf"))
            { 
                FileUploaderRequest fileUploaderRequest = new FileUploaderRequest();
                fileUploaderRequest.contentType = mVSetting.fileToUpload.ContentType;
                fileUploaderRequest.fileExtention = Path.GetExtension(mVSetting.fileToUpload.FileName);
                fileUploaderRequest.userId = GetCurrentUserId();
                fileUploaderRequest.id = Convert.ToInt32(model.id);
                fileUploaderRequest.sourceController = "Setting";

                using (var memoryStream = new MemoryStream())
                {
                    await mVSetting.fileToUpload.CopyToAsync(memoryStream);
                    fileUploaderRequest.content = Convert.ToBase64String(memoryStream.ToArray());
                }
                await fileUploadService.FileUpload(fileUploaderRequest);
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