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
    public class CouponController : Controller
    {
        private readonly ILogger<CouponController> _logger;
        private readonly IService<Coupon> service;
        private readonly string url = "/api/Coupon/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public CouponController(IService<Coupon> _service, ILogger<CouponController> logger)
        {
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<Coupon> coupons = await service.GetAll(url, await GetToken());
                return View(coupons);
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
            return View(new MVCoupon { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MVCoupon mVCoupon)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    Coupon coupon = new Coupon()
                    {
                        id = mVCoupon.id,
                        couponName = mVCoupon.couponName,
                        code = mVCoupon.code,
                        discountType = mVCoupon.discountType,
                        value = mVCoupon.value,
                        numberOfUse = mVCoupon.numberOfUse,
                        startOn = mVCoupon.startOn,
                        expireOn = mVCoupon.expireOn,
                        maximumDiscount = mVCoupon.maximumDiscount,
                        minimumSpend = mVCoupon.minimumSpent,
                        isActive = mVCoupon.isActive,
                        createdBy = mVCoupon.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId(),
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now
                    };
                    model = await service.Post(coupon, url, await GetToken());

                    if (model.flag)
                    {
                        StatusMessage = $"Coupon Id {model.id} has been Added.";
                        return RedirectToAction("Details", new { id = model.id });
                    }
                    else
                    {

                        StatusMessage = $"{model.message.ToString()}";
                        ModelState.AddModelError("", model.message);
                        return View(mVCoupon);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
            }
            return View(mVCoupon);
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
                Coupon alert = await service.Get(id, url, await GetToken());
                return View(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            try
            {
                POJO model = await service.Delete(id, url, await GetToken());
                StatusMessage = $"Coupon Id {id} has been deleted.";
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
                Coupon coupon = await service.Get(id, url, await GetToken());

                MVCoupon mVCoupon = new MVCoupon()
                {
                    id = coupon.id,
                    couponName = coupon.couponName,
                    code = coupon.code,
                    discountType = coupon.discountType,
                    value = coupon.value,
                    numberOfUse = coupon.numberOfUse,
                    startOn = coupon.startOn,
                    expireOn = coupon.expireOn,
                    maximumDiscount = coupon.maximumDiscount,
                    minimumSpent = coupon.minimumSpend,
                    isActive = coupon.isActive,
                    createdBy = coupon.id.Equals(0) ? GetCurrentUserId() : null,
                    modifiedBy = GetCurrentUserId(),
                    createDate = DateTime.Now,
                    updateDate = DateTime.Now
                };

                return View("Save", mVCoupon);
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

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsExistCouponCode(string code)
        {
            var coupon = await service.Get(code, $"{url}GetByCode/", await GetToken());
            if (coupon == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"The Code {coupon.code} is already exist, used as {coupon.couponName}.");
            }
        }

    }
}