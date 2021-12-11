using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class SocialMediaViewComponent : ViewComponent
    {
        private readonly IService<Setting> service;
        private readonly string url = "/api/setting/";

        public SocialMediaViewComponent(IService<Setting> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var setting = await service.GetAll(url, await GetToken());
            return View(setting?.FirstOrDefault());
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
