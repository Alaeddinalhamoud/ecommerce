using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class PageViewComponent : ViewComponent
    {
        private readonly IService<IEnumerable<Page>> service;
        private readonly string url = "/api/page/PageByNav/";

        public PageViewComponent(IService<IEnumerable<Page>> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string navId)
        {
            var pages = await service.Get(navId, url, await GetToken());
            return View(pages);
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
