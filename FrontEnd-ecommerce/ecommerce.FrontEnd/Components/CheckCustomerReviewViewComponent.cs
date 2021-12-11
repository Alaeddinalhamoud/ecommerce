using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class CheckCustomerReviewViewComponent : ViewComponent
    {
        private readonly IService<Review> service;
        private readonly string url = "/api/Review/CheckCustomerIsReview/";

        public CheckCustomerReviewViewComponent(IService<Review> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId, int productId)
        {
            var alerts = await service.Post(new Review() {createdBy = userId, productId = productId  }, url, await GetToken());
            return View(alerts);
        }
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }

    }
}
