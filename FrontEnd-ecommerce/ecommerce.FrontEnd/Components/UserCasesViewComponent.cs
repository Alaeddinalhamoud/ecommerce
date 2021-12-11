using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class UserCasesViewComponent : ViewComponent
    {
        private readonly IService<MVReportUserCases> service;
        private readonly string url = "/api/Report/GetUserCases/";

        public UserCasesViewComponent(IService<MVReportUserCases> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var cases = await service.Get(userId, url, await GetToken());
            return View(cases);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }

    }
}
