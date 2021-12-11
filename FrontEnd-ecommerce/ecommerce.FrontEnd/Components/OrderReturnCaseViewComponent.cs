using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent, Authorize]
    public class OrderReturnCaseViewComponent : ViewComponent
    {
        private readonly IService<OrderReturnCase> service;
        private readonly string url = "/api/OrderReturnCase/GetOrderReturnCaseByOrder/";

        public OrderReturnCaseViewComponent(IService<OrderReturnCase> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string orderId)
        {
            var orderReturnCase = await service.Get(orderId, url, await GetToken());
            return View(orderReturnCase);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
