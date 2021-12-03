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
    public class CouponHistoryController : Controller
    {
        private readonly IService<CouponHistory> service;
        private readonly string url = "/api/CouponHistory/";
        private readonly IService<Coupon> serviceCoupon;
        private readonly string urlCoupon = "/api/Coupon/";
        private readonly ILogger<CouponHistoryController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public CouponHistoryController(IService<CouponHistory> _service, ILogger<CouponHistoryController> logger, IService<Coupon> _serviceCoupon)
        {
            service = _service;
            _logger = logger;
            serviceCoupon = _serviceCoupon;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<CouponHistory> couponHistories = await service.GetAll(url, await GetToken());
                IQueryable<Coupon> coupons = await serviceCoupon.GetAll(urlCoupon, await GetToken());

                var couponHistoryDetails = from couponHistory in couponHistories
                                           join coupon in coupons on couponHistory.couponId equals coupon.id
                                           select new MVCouponHistoryDetails
                                           {
                                               couponId = coupon.id,
                                               couponCode = coupon.code,
                                               orderId = couponHistory.orderId,
                                               Date = couponHistory.createDate
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