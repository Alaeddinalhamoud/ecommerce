using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Components
{
    [ViewComponent]
    public class MonthlyCountReportViewComponent : ViewComponent
    {
        private readonly IService<MVReportNumberOfProductReview> service;
        private readonly string url = "/api/Report/GetMonthlyCount/";

        public MonthlyCountReportViewComponent(IService<MVReportNumberOfProductReview> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string year)
        {
            var mVReportNumberOfProductReview = await service.Get(year, url, await GetToken());
            return View(mVReportNumberOfProductReview);
        }
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
