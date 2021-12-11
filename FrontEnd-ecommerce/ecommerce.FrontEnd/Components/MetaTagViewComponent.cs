using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class MetaTagViewComponent : ViewComponent
    {
        private readonly IService<MetaTag> service;
        private readonly string url = "/api/metatag/";

        public MetaTagViewComponent(IService<MetaTag> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string metaTagId)
        {
            var metaTag = await service.Get(metaTagId, url, await GetToken());
            return View(metaTag);
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
