using System; 
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {        
        private readonly IService<Review> service;
        private readonly string url = "/api/review/";
        private readonly ILogger<ReviewController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public ReviewController(IService<Review> _service, ILogger<ReviewController> logger)
        {
            service = _service;
            _logger = logger;
        }

        //load all unapproved reviews.
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<Review> reviews = await service.GetAll($"{url}GetReviewsNeedApprove", await GetToken());
                return View(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }          
        }

        public async Task<IActionResult> ApproveReview(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            Review review = new Review()
            {
                id = id,
                isApproved = true,
                updateDate = DateTime.Now,
                modifiedBy = GetCurrentUserId()
            };

            try
            {
                POJO model = await service.Post(review, url, await GetToken());
                StatusMessage = $"Review Id {id} has been aproved.";              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Index", "Review");
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