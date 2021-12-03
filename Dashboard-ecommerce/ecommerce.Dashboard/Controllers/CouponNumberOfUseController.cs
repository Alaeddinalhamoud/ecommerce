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
    public class CouponNumberOfUseController : Controller
    {
        private readonly IService<CouponNumberOfUse> service;
        private readonly string url = "/api/CouponNumberOfUse/";
        private readonly IService<Coupon> serviceCoupon;
        private readonly string urlCoupon = "/api/Coupon/";
        private readonly ILogger<CouponNumberOfUseController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public CouponNumberOfUseController(IService<CouponNumberOfUse> _service, IService<Coupon> _serviceCoupon,
            ILogger<CouponNumberOfUseController> logger)
        {
            service = _service;
            serviceCoupon = _serviceCoupon;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<CouponNumberOfUse> couponNumberOfUses = await service.GetAll(url, await GetToken());
                IQueryable<Coupon> coupons = await serviceCoupon.GetAll(urlCoupon, await GetToken());

                var couponHistoryDetails = from couponNumberOfUse in couponNumberOfUses
                                           join coupon in coupons on couponNumberOfUse.couponId equals coupon.id
                                           select new MVCouponNumberOfUseDetails
                                           {
                                               couponId = coupon.id,
                                               couponCode = coupon.code,
                                               numberOfUse = couponNumberOfUse.numberOfUse,
                                               Date = couponNumberOfUse.createDate
                                           };


                return View(couponHistoryDetails);
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