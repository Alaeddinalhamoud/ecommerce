using System;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData; 
using ecommerce.FrontEnd.Models;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        //Calling  services
        private readonly IService<CartLine> serviceCartLine; 
        private readonly IService<CartLineDetails> serviceCartLineDetails;
        private readonly string urlCartLine = "/api/CartLine/";
        private readonly IService<CheckOut> serviceCheckOut;
        private readonly string url = "/api/Cart/";

        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public CartController(IService<CartLine> _serviceCartLine,
            IService<CartLineDetails> _serviceCartLineDetails,
            IService<CheckOut> _serviceCheckOut, ILogger<CartController> logger)
        {
            serviceCartLine = _serviceCartLine;
            serviceCartLineDetails = _serviceCartLineDetails;
            serviceCheckOut = _serviceCheckOut;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index","Home");
        }

        public IActionResult RedirectToProduct(string id)
        {
            return RedirectToAction("Details", "Product", new { id = id });
        }
        //Add to cart button productId
        [HttpPost, ValidateAntiForgeryToken]        
        public async Task<IActionResult> AddToCart(MVCart mVCart)
        {
            POJO model = new POJO();
            CartLine numberOfItemsInCart = new CartLine();
            CartLine cartLine = new CartLine()
            { 
                //Price will be included in the API
                productId = mVCart.productNum,
                qty = mVCart.qty,                
                createDate = DateTime.Now,
                createdBy = GetCurrentUserId(),
                modifiedBy = GetCurrentUserId()                
            };
            try
            {
                //Will returen the cart id
                model = await serviceCartLine.Post(cartLine, $"{url}AddToCart", await GetToken());
                //after add to cart need to  update the navbar,
                // by reading how many product for the current user cart (Opened by userId) table and update the nav.
                numberOfItemsInCart = await serviceCartLine.Get(GetCurrentUserId(), $"{url}NumberOfProducts/", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Model error:{model.message}, UserId: {GetCurrentUserId()}");
                return Content("0");
            }
            return Content(numberOfItemsInCart.id.ToString());
        }

        //Add to cart button productId only single product
        [HttpGet]
        public async Task<IActionResult> AddSingleToCart(int id)
        {
            POJO model = new POJO();
            CartLine numberOfItemsInCart = new CartLine();
            CartLine cartLine = new CartLine()
            {
                //Price will be included in the API
                productId = id,
                qty = 1,
                createDate = DateTime.Now,
                createdBy = GetCurrentUserId(),
                modifiedBy = GetCurrentUserId()
            };
            try
            {
                model = await serviceCartLine.Post(cartLine, $"{url}AddToCart", await GetToken());
                //after add to cart need to  update the navbar, by UserId
                // by reading how many product for the current user cart (Opened by userId) table and update the nav.
                numberOfItemsInCart = await serviceCartLine.Get(GetCurrentUserId(), $"{url}NumberOfProducts/", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Model error:{model.message}, UserId: {GetCurrentUserId()}");
                return Content("0");
            }
            return Content(numberOfItemsInCart.id.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"> Is the product Id</param>
        /// <param name="currentQty"> Is th current Qty</param>
        /// <param name="QtyOpration"> In case if need to increse or decrese the customer will send (-1/1)</param>
        /// <returns></returns>
        [HttpGet, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ModifyProductQty(int id, int currentQty, int qtyOpration, int cartLineId)
        {
            POJO model = new POJO();
          
            try
            {
                //If the currentQty = 1 than need to delete the product from the cart, by calling remove from cart
                if (currentQty.Equals(1) && qtyOpration.Equals(-1) || currentQty.Equals(0))
                {
                    model = await serviceCartLine.Delete(cartLineId, urlCartLine, await GetToken());
                    model.id = "0";
                }
                else
                {
                    CartLine numberOfItemsInCart = new CartLine();
                    CartLine cartLine = new CartLine()
                    {
                        //Price will be included in the API
                        id = cartLineId,
                        productId = id,
                        qty = qtyOpration,
                        createDate = DateTime.Now,
                        createdBy = GetCurrentUserId(),
                        modifiedBy = GetCurrentUserId()
                    }; 
                    //Will returen the cart id
                    model = await serviceCartLine.Post(cartLine, $"{url}AddToCart", await GetToken());
                    //after add to cart need to  update the navbar,
                    // by reading how many product for the current user cart (Opened by userId) table and update the nav.
                    //  numberOfItemsInCart = await serviceCartLine.Get(GetCurrentUserId(), $"{url}NumberOfProducts/", await GetToken());
                }  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Model error:{model.message}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            StatusMessage = $"The Cart has been Updated successfuly.";
            return RedirectToAction("MyCart");
        }

        [HttpGet]
        public async Task<IActionResult> MyCart()
        {
            CartLineDetails cartLineDetails = null;
            try
            {
               cartLineDetails = await serviceCartLineDetails.Get(GetCurrentUserId(), $"{url}GetMyCart/", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return View(cartLineDetails);
            }
            return View(cartLineDetails);
        }
        //CartLineId
        [HttpGet]
        public async Task<IActionResult> RemoveFromMyCart(int id)
        {
            POJO model = new POJO();
            if(id.Equals(0))
            {
                _logger.LogError($"Empty Data, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            try
            {
                model = await serviceCartLine.Delete(id, urlCartLine, await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Model error:{model.message}, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            StatusMessage = $"The product has been removed successfuly.";
            return RedirectToAction("MyCart");
        }


        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            try
            {
               var checkOut = await serviceCheckOut.Get(GetCurrentUserId(), $"{url}checkOut/", await GetToken());

                if(checkOut.total.Equals(0))
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(checkOut);
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