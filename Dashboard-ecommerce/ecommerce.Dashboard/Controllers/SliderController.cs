using System; 
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Dashboard.Models;
using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UtilityFunctions.Data;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    { 
        private readonly IService<Slider> service;
        private readonly string url = "/api/slider/";
        private readonly ILogger<SliderController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public SliderController(IService<Slider> _service, ILogger<SliderController> logger)
        { 
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<Slider> sliders = await service.GetAll(url, await GetToken());
                return View(sliders?.OrderByDescending(d => d.createDate));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View(new MVSlider { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MVSlider mVSlider)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                {
                    Slider slider = new Slider()
                    {
                        id = mVSlider.id,
                        name = mVSlider.name,
                        description = mVSlider.description, 
                        createdBy = mVSlider.id.Equals(0) ? GetCurrentUserId() : null,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        modifiedBy = GetCurrentUserId(),
                        link = mVSlider.link,
                        buttonCaption = mVSlider.buttonCaption,
                        isEnabled = mVSlider.isEnabled
                    };

                    if(mVSlider.id == 0)
                    {
                       slider.imagePath = "/img/NoImage.jpg";
                    }
                    
                    model = await service.Post(slider, url, await GetToken());

                    if (mVSlider.updatelogo && model.flag)
                    {
                        return RedirectToAction("Upload", "Media", new FileUploaderRequest() { id = Convert.ToInt32(model.id), sourceController = "Slider" });
                    }

                    if (model.flag && !mVSlider.updatelogo)
                    {
                        StatusMessage = $"Slider Id {model.id} has been Added.";
                        return RedirectToAction("Details", new { id = model.id });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                ModelState.AddModelError("", model.message);
            }
            return View(mVSlider);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {
                Slider slider  = await service.Get(id, url, await GetToken());
                return View(slider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            try
            {
                Slider slider = await service.Get(id, url, await GetToken());
                MVSlider mVSlider = new MVSlider()
                {
                    id = slider.id,
                    name = slider.name,
                    description = slider.description,
                    link = slider.link
                };
                return View("Save", mVSlider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        } 
         
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            } 

            try
            {
                POJO model = await service.Delete(id, url, await GetToken());
                StatusMessage = $"Slider Id {model.id} has been Deteted.";
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public string GetCurrentUserId()
        {
            return User?.FindFirst(c => c.Type == "sub")?.Value;
        }
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}