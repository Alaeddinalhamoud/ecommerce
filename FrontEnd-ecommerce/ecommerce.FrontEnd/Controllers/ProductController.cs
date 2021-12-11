using System;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        private readonly string url = "/api/product/";      
      
        private readonly IService<ProductDetail> serviceDetails;
        //calling Recently Visisted Product
        private readonly IService<RecentlyViewedProduct> serviceRecentlyViewedProduct;
        private readonly string urlRecentlyViewedProduct = "/api/RecentlyViewedProduct/";
        //calling Most Viewed Product
        private readonly IService<MostViewedProduct> serviceMostViewedProduct;
        private readonly string urlMostViewedProduct = "/api/MostViewedProduct/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public ProductController(IService<ProductDetail> _serviceDetails,
            IService<RecentlyViewedProduct> _serviceRecentlyViewedProduct,
            IService<MostViewedProduct> _serviceMostViewedProduct, ILogger<ProductController> logger)
        {
            serviceDetails = _serviceDetails;
            serviceRecentlyViewedProduct = _serviceRecentlyViewedProduct;
            serviceMostViewedProduct = _serviceMostViewedProduct;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [Route("product/{id}/{title}")]
        [Route("product/{id}")]
        [Route("/Product/Details/{id}")]
        public async Task<IActionResult> Details(string id, string title)
        {
            if (String.IsNullOrEmpty(id))
            {
                _logger.LogError($"Empty Data, UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {
                ProductDetail productDetail = await serviceDetails.Get(id, $"{url}GetProductDetailById/", await GetAccessToken()); 
              
                if(productDetail != null)
                {

                    //Add to recently viewed tb.      ****Dangers (Neeed to be scured by api key) 
                    if (GetCurrentUserId() != null)
                    {
                        //Update product freq (Most viewed Product)
                        // (We have added new Tb for mostViewed Product) await service.Post(new Product() { id = productDetail.id, frequency = productDetail.frequency + 1, isApproved = true, isDeleted = false  }, url, await GetToken());
                        await serviceMostViewedProduct.Post(new MostViewedProduct() {productId = Convert.ToInt32(id), frequency = 1, lastVisitDate = DateTime.Now },urlMostViewedProduct, await GetToken());
                        await serviceRecentlyViewedProduct.Post(new RecentlyViewedProduct() { productId = Convert.ToInt32(id),
                                               userId = GetCurrentUserId(), createDate = DateTime.Now }, urlRecentlyViewedProduct,await GetToken());
                    }                   
                }
                return View(productDetail);
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId() == null ? "Unregestred user" : GetCurrentUserId();
                _logger.LogError(ex, $"UserId: {userId}");
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

        private async Task<string> GetAccessToken()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Medi_Access")))
            {
                HttpContext.Session.SetString("_Medi_Access", await serviceDetails.GetAccessToken());
            }
            return HttpContext.Session.GetString("_Medi_Access");
        }
    }
}