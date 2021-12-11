using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class OrderOpenCasesViewComponent : ViewComponent
    {
        private readonly IService<MVReportUserCases> service;
        private readonly string url = "/api/Report/GetUserOpenCasesByOrderId/";

        public OrderOpenCasesViewComponent(IService<MVReportUserCases> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string orderId)
        {
            var cases = await service.Get(orderId, url, await GetToken());
            return View(cases);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }

    }
}
