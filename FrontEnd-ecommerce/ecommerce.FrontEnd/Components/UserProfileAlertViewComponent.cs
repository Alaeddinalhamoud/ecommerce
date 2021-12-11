using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class UserProfileAlertViewComponent : ViewComponent
    {
        private readonly IService<Alert> service;
        private readonly string url = "/api/alert/UserProfileAlert/";

        public UserProfileAlertViewComponent(IService<Alert> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var alerts = await service.Get(userId, url, await GetToken());
            return View(alerts);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }

    }
}
