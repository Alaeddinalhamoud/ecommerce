using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class LayoutCategoriesViewComponent : ViewComponent
    {
        private readonly IService<Category> service;
        private readonly string url = "/api/category/";
        
        public LayoutCategoriesViewComponent(IService<Category> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await service.GetAll(url, await GetToken());
            return View(categories);
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
