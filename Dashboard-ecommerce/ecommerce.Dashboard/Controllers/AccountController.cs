using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Dashboard.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }
        public string GetCurrentUserId()
        {
            return User?.FindFirst(c => c.Type == "sub")?.Value;
        } 
    }
}