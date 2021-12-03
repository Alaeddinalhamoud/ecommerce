using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VendorProfileController : Controller
    {
        private readonly IService<VendorProfile> serviceVendorProfiles;
        private readonly IService<VendorProfileDetails> serviceVendorProfile;
        private readonly string url = "/api/VendorProfile/";
        private readonly ILogger<VendorProfileController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public VendorProfileController(IService<VendorProfile> _serviceVendorProfiles,
                                       IService<VendorProfileDetails> _serviceVendorProfile, ILogger<VendorProfileController> logger)
        {
            serviceVendorProfiles = _serviceVendorProfiles;
            serviceVendorProfile = _serviceVendorProfile;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<VendorProfile> vendorProfile = await serviceVendorProfiles.GetAll($"{url}", await GetToken());
                //Just show the Pending
                return View(vendorProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                VendorProfileDetails vendorProfile = await serviceVendorProfile.Get(id.ToString(), $"{url}GetVendorProfileDetails/", await GetToken());
                //Just show the Pending
                return View(vendorProfile);
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
