using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class MiniCartViewComponent : ViewComponent
    {
        private readonly IService<CartLineDetails> serviceCartLineDetails;
        private readonly string url = "/api/Cart/";

        public MiniCartViewComponent(IService<CartLineDetails> _serviceCartLineDetails)
        {
            serviceCartLineDetails = _serviceCartLineDetails;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
           var cartLineDetails = await serviceCartLineDetails.Get(userId, $"{url}GetMyCart/", await GetToken());
            return View(cartLineDetails);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        } 
    }
}
