using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class ShopCategoriesViewComponent : ViewComponent
    {
        private readonly IService<ShopCategories> service;
        private readonly string url = "/api/shop/ShopCategories/";
        
        public ShopCategoriesViewComponent(IService<ShopCategories> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var shopCategories = await service.Get("0", url, await GetToken());
            return View(shopCategories);
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
