using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class WishlistStatusViewComponent : ViewComponent
    {
        private readonly IService<Wishlist> service;
        private readonly string url = "/api/Wishlist/wishlistStatus";

        public WishlistStatusViewComponent(IService<Wishlist> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId, int productId)
        {
            Wishlist wishlist = new Wishlist()
            {
                id = 0,                
                userId = userId,
                productId = productId
            };
            var wishlistStatus = await service.Post(wishlist, url, await GetToken());
            return View(wishlistStatus.flag);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
