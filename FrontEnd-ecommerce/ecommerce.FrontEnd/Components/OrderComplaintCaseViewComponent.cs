using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class OrderComplaintCaseViewComponent : ViewComponent
    {
        private readonly IService<OrderComplaintCase> service;
        private readonly string url = "/api/OrderComplaintCase/GetOrderComplaintCaseByOrder/";

        public OrderComplaintCaseViewComponent(IService<OrderComplaintCase> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string orderId)
        {
            var orderComplaintCase = await service.Get(orderId, url, await GetToken());
            return View(orderComplaintCase);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
