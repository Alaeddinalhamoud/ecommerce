using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class MostViewedProductsViewComponent : ViewComponent
    {
        private readonly IService<ProductDetail> service;
        private readonly string url = "/api/MostViewedProduct/";
        
        public MostViewedProductsViewComponent(IService<ProductDetail> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mostViwedProdcuts = await service.GetAll(url, await GetToken());
            return View(mostViwedProdcuts);
        }

        private async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Medi_Access")))
            {
                HttpContext.Session.SetString("_Medi_Access", await service.GetAccessToken());
            }
            return HttpContext.Session.GetString("_Medi_Access");
        }
    }
}
