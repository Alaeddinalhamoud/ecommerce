using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class YourCartLinesViewComponent : ViewComponent
    {
        private readonly IService<CartLineDetails> serviceCartLineDetails;
        private readonly string url = "/api/Cart/";

        public YourCartLinesViewComponent(IService<CartLineDetails> _serviceCartLineDetails)
        {
            serviceCartLineDetails = _serviceCartLineDetails;
        }

        public async Task<IViewComponentResult> InvokeAsync(string cartId)
        {
           var cartLineDetails = await serviceCartLineDetails.Get(cartId, $"{url}GetCart/", await GetToken());
            return View(cartLineDetails);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        } 
    }
}
