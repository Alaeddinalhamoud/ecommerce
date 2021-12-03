using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Components
{
    [ViewComponent]
    public class PaymentMethodReportViewComponent : ViewComponent
    {
        private readonly IService<MVReportNumberOfPaymentMethod> service;
        private readonly string url = "/api/Report/GetPaymentMethodNumber/";

        public PaymentMethodReportViewComponent(IService<MVReportNumberOfPaymentMethod> _service)
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
