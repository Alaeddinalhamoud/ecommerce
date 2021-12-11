using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class RecentlyViewedProductsViewComponent : ViewComponent
    {
        private readonly IService<IEnumerable<RecentlyViewedProductDetail>> service;
        private readonly string url = "/api/RecentlyViewedProduct/GetRecentlyViewedProducts/";

        public RecentlyViewedProductsViewComponent(IService<IEnumerable<RecentlyViewedProductDetail>> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string UserId)
        {
            var productReviews = await service.Get(UserId, url, await GetToken());
            return View(productReviews);
        } 
        
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
