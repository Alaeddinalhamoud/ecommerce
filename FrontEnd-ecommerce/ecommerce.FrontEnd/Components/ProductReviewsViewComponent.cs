using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class ProductReviewsViewComponent : ViewComponent
    {
        private readonly IService<IEnumerable<Review>> service;
        private readonly string url = "/api/Review/GetProductReviews/";

        public ProductReviewsViewComponent(IService<IEnumerable<Review>> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string productId)
        {
            var productReviews = await service.Get(productId, url, await GetToken());
            return View(productReviews);
        }

        private async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Medi_Access")))
            {
                HttpContext.Session.SetString("_Medi_Access", await service.GetAccessToken());
            }
            return HttpContext.Session.GetString("_Medi_Access");
        }
    }
}
