using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class ShippingAddressViewComponent : ViewComponent
    {
        private readonly IService<IEnumerable<Address>> service;
        private readonly string url = "/api/address/GetAddressesByUserId/";

        public ShippingAddressViewComponent(IService<IEnumerable<Address>> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var addresses = await service.Get(userId, url, await GetToken());
            return View(addresses);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }

    }
}
