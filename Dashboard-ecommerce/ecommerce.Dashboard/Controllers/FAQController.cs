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

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles ="Admin")]
    public class FAQController : Controller
    { 
        private readonly IService<FAQ> service;
        private readonly string url = "/api/faq/";
        private readonly ILogger<FAQController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public FAQController(IService<FAQ> _service, ILogger<FAQController> logger)
        { 
            service = _service;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            { 
                IQueryable<FAQ> fAQs =await service.GetAll(url, await GetToken());
                return View(fAQs);
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
            return View(new MVFAQ { id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Save(MVFAQ mVFAQ)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                { 
                    FAQ fAQ = new FAQ()
                    {
                        id = mVFAQ.id,
                        question = mVFAQ.question,
                        answer = mVFAQ.answer,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        createdBy = mVFAQ.id.Equals(0) ? GetCurrentUserId() : null,
                        modifiedBy = GetCurrentUserId()
                    };
                    
                    model = await service.Post(fAQ, url, await GetToken()); 
                    StatusMessage = $"FAQ Id {model.id} has been Added/Updated.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                
                return RedirectToAction("index");
            }
            ModelState.AddModelError("", model.message);
            return View(mVFAQ);
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
                StatusMessage = $"FAQ Id {id} has been deleted.";
                return RedirectToAction("index");
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
            if(String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            { 
                FAQ fAQ = await service.Get(id, url, await GetToken());
                MVFAQ mVFAQ = new MVFAQ()
                {
                    id = fAQ.id,
                    question = fAQ.question,
                    answer = fAQ.answer                    
                };
                return View("Save", mVFAQ);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if(String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            { 
                FAQ fAQ = await service.Get(id, url, await GetToken());
                return View(fAQ);
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