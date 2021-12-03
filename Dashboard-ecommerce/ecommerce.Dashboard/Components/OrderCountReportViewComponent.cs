using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Components
{
    [ViewComponent]
    public class OrderCountReportViewComponent : ViewComponent
    {
        private readonly IService<MVReportNumberOfOrdersMonthly> service;
        private readonly string url = "/api/Report/GetOrderCountMonthly/";

        public OrderCountReportViewComponent(IService<MVReportNumberOfOrdersMonthly> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string year)
        {
            var mVReportNumberOfOrdersMonthly = await service.Get(year, url, await GetToken());
            return View(mVReportNumberOfOrdersMonthly);
        }
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
