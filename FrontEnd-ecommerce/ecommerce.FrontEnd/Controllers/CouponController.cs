using System;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.Helper;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class CouponController : Controller
    {
        private readonly ILogger<CouponController> _logger;
        private readonly IService<Coupon> service;
        private readonly string url = "/api/Coupon/";
        private readonly IService<CartLineDetails> serviceCartLineDetails;
        private readonly IService<Cart> serviceCart;
        private readonly string urlCart = "/api/Cart/";
        private readonly IService<CouponNumberOfUse> serviceCouponNumberOfUse;
        private readonly string urlCouponNumberOfUse = "/api/CouponNumberOfUse/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public CouponController(IService<Coupon> _service, IService<CartLineDetails> _serviceCartLineDetails,
            IService<Cart> _serviceCart, IService<CouponNumberOfUse> _serviceCouponNumberOfUse,
            ILogger<CouponController> logger)
        {
            service = _service;
            serviceCartLineDetails = _serviceCartLineDetails;
            serviceCart = _serviceCart;
            serviceCouponNumberOfUse = _serviceCouponNumberOfUse;
            _logger = logger;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CouponChecker(string code)
        {
            POJO model = new POJO();
            if (string.IsNullOrEmpty(code))
            {
                StatusMessage = $"Error Please add Coupon to the box.";
                return RedirectToAction("MyCart", "cart");
            }
            try
            {
                //Get the Coupon details
                var coupon = await service.Get(code, $"{url}GetByCode/", await GetToken());
                if (coupon is null)
                {
                    StatusMessage = $"Error The Coupon {code} is not exist.";
                    return RedirectToAction("MyCart", "cart");
                }
                var couponNumberOfUser = await serviceCouponNumberOfUse.Get(coupon.id.ToString(), $"{urlCouponNumberOfUse}GetByCouponId/", await GetToken());
                if (couponNumberOfUser is null)
                {
                    couponNumberOfUser = new CouponNumberOfUse { numberOfUse = 0 };
                }
                var cartLineDetails = await serviceCartLineDetails.Get(GetCurrentUserId(), $"{urlCart}GetMyCart/", await GetToken());
                //get the current cart total  by userId
                var totalCart = TotalCart.GetTotalCart(cartLineDetails).total;

                //check coupon
                //1- date
                //2- min speand
                if (!coupon.isActive)
                {
                    StatusMessage = $"Error The coupon {code} does not available anymore.";
                }
                else if (coupon.startOn > DateTime.Now.Date)
                {
                    StatusMessage = $"Error The coupon {code} is not allowed to be used.";
                }
                else if (coupon.expireOn < DateTime.Now.Date)
                {
                    StatusMessage = $"Error The coupon {code} is expired.";
                }
                else if (coupon.minimumSpend > totalCart)
                {
                    StatusMessage = $"Error You cart total amount {totalCart} USD is less than {coupon.minimumSpend} USD.";
                }
                else if (coupon.numberOfUse < couponNumberOfUser.numberOfUse)
                {
                    StatusMessage = $"Error The Coupon {coupon.code} is out of limit";
                }
                else
                {
                    StatusMessage = $"The coupon {code} has been added successfuly.";
                    //3- add to the cartTable
                    var cartModel = await serviceCart.Post(new Cart { id = cartLineDetails.cartId, couponId = coupon.id, modifiedBy = GetCurrentUserId() }, urlCart, await GetToken());
                }

                return RedirectToAction("MyCart", "Cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Model Error {model.message}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CouponRemove(int id)
        {
            POJO model = new POJO();
            if (id == 0)
            {
                StatusMessage = $"Empty request.";
                return RedirectToAction("MyCart", "Cart");
            }
            try
            {
                //get the cart and check if the user secure.
                var cart = await serviceCart.Get(id.ToString(), urlCart, await GetToken());
                if (cart.createdBy.Equals(GetCurrentUserId()))
                {
                    StatusMessage = $"The coupon has been removed successfuly.";
                    var cartModel = await serviceCart.Post(new Cart { id = id, couponId = 0, modifiedBy = GetCurrentUserId() }, urlCart, await GetToken());
                }
                else
                {
                    StatusMessage = $"This not your cart.";
                }

                return RedirectToAction("MyCart", "Cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Model Error {model.message}, UserId: {GetCurrentUserId()}");
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