using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class WishListController : Controller
    {
        private readonly ILogger<WishListController> _logger;
        //Calling  services
        private readonly IService<Wishlist> service;
        private readonly string url = "/api/Wishlist/";
        private readonly IService<IEnumerable<WishlistProduct>> serviceWishlistProduct;

        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public WishListController(IService<Wishlist> _service,
            IService<IEnumerable<WishlistProduct>> _serviceWishlistProduct, ILogger<WishListController> logger)
        {
            service = _service;
            serviceWishlistProduct = _serviceWishlistProduct;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AddToWishlist(int id)
        {
            POJO model = new POJO();
            Wishlist numberOfItems = new Wishlist();
            Wishlist wishlist = new Wishlist()
            {
                productId = id,
                userId = GetCurrentUserId(),
                createDate = DateTime.Now,
                createdBy = GetCurrentUserId(),
                modifiedBy = GetCurrentUserId()
            };
            try
            {
                model = await service.Post(wishlist, url, await GetToken());
                //after add to wish list need to  update the navbar,
                // by reading how many product for the current user wishlist table and update the nav.
                numberOfItems = await service.Get(GetCurrentUserId(), $"{url}NumberOfProducts/", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $" Error Model : {model.message}, UserId: {GetCurrentUserId()}");
                return Content("0");
            }
            return Content(numberOfItems.id.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> MyWishlist()
        {
            IEnumerable<WishlistProduct> wishlists = null;
            try
            {
                wishlists = await serviceWishlistProduct.Get(GetCurrentUserId(), $"{url}GetMyWishlist/", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(wishlists);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveFromMyWishlist(int id)
        {
            POJO model = new POJO();
            try
            {
                model = await service.Delete(id, url, await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: {model.message}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            StatusMessage = $"The product has been removed successfuly.";
            return RedirectToAction("MyWishlist");
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