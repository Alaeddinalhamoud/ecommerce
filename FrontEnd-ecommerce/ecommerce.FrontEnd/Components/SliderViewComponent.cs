using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Components
{
    [ViewComponent]
    public class SliderViewComponent : ViewComponent
    {
        private readonly IService<IEnumerable<Slider>> service;
        private readonly string url = "/api/slider/NumberOfSliders/";

        public SliderViewComponent(IService<IEnumerable<Slider>> _service)
        {
            service = _service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string numberOfSliders)
        {
            var sliders = await service.Get(numberOfSliders, url, await GetToken());
            return View(sliders);
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
