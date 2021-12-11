using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class RelatedProductsViewComponent : ViewComponent
    {
        private readonly IService<IEnumerable<ProductDetail>> service;
        private readonly string url = "/api/product/RelatedProduts/";

        public RelatedProductsViewComponent(IService<IEnumerable<ProductDetail>> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string CategoryId)
        {
            var mostViwedProdcuts = await service.Get(CategoryId, url, await GetToken());
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
