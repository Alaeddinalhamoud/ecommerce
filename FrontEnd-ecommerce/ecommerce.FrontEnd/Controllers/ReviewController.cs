using System; 
using System.Threading.Tasks;
using ecommerce.Data; 
using ecommerce.FrontEnd.Models;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ILogger<ReviewController> _logger;
        //Calling  services
        private readonly IService<Review> service;
        private readonly string url = "/api/review/";

        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public ReviewController(IService<Review> _service, ILogger<ReviewController> logger)
        {
            service = _service;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(MVReview mVReview)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                Review review = new Review()
                {
                    rating = mVReview.rating,
                    name = mVReview.reviewerName,
                    email = mVReview.reviewerEmail,
                    description = mVReview.reviewDescription,
                    productId = mVReview.productId,
                    createDate = DateTime.Now,
                    createdBy = GetCurrentUserId(),
                    modifiedBy = GetCurrentUserId()
                };

                try
                {
                    model = await service.Post(review, url, await GetToken());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return PartialView("_Failure", "Error");
                }

                ModelState.AddModelError("", model.message);
            }
            return PartialView("_Success");
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