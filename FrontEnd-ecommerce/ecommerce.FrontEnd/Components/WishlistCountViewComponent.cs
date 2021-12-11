using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class WishlistCountViewComponent : ViewComponent
    {
        private readonly IService<Wishlist> service;
        private readonly string url = "/api/Wishlist/";

        public WishlistCountViewComponent(IService<Wishlist> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
          var  numberOfItems = await service.Get(userId, $"{url}NumberOfProducts/", await GetToken());
            return View(numberOfItems.id);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
