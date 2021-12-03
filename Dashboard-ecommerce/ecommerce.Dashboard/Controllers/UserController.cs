using System; 
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Dashboard.Models; 
using ecommerce.Data;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly FileUploadService fileUploadService;
        private readonly IService<ApplicationUser> service;
        private readonly IService<User> serviceUsers;
        private readonly string url = "/api/user/";
        private readonly ILogger<UserController> _logger;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }
        public UserController(FileUploadService _fileUploadService,
                              IService<ApplicationUser> _service,
                              IService<User> _serviceUsers, ILogger<UserController> logger)
        {
            fileUploadService = _fileUploadService;
            service = _service;
            serviceUsers = _serviceUsers;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<User> users = await serviceUsers.GetAll(url, await GetToken());
                return View(users);
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
                ApplicationUser applicationUser = await service.Get(id, url, await GetToken());
                MvApplicationUser mVApplicationUser = new MvApplicationUser()
                {
                    name = applicationUser.name,
                    email = applicationUser.Email,
                    phone = applicationUser.PhoneNumber,
                    dOB = applicationUser.dOB,
                    gender = applicationUser.gender,
                    isBlocked = applicationUser.isBlocked,
                    isVendor = applicationUser.isVendor,
                    isAdmin = applicationUser.isAdmin
                };
                return View(mVApplicationUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MvApplicationUser mvApplicationUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser applicationUser = new ApplicationUser()
                    {
                        Id = mvApplicationUser.id,
                        name = mvApplicationUser.name,
                        Email = mvApplicationUser.email,
                        dOB = mvApplicationUser.dOB,
                        isBlocked = mvApplicationUser.isBlocked,
                        isVendor = mvApplicationUser.isVendor,
                        isAdmin = mvApplicationUser.isAdmin,
                        PhoneNumber = mvApplicationUser.phone,
                        modifiedBy = GetCurrentUserId(),
                        updateDate = DateTime.Now
                    };
                    POJO model = await service.Post(applicationUser, url, await GetToken());
                    StatusMessage = $"User Id {model.id} has been Updated.";
                    return RedirectToAction("Details", new { id = model.id });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }

            }
            return View(mvApplicationUser);
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
                ApplicationUser applicationUser = await service.Get(id, url, await GetToken());
                return View(applicationUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }
       
        public async Task<IActionResult> BlockUser(string id)
        {
            try
            {
                ApplicationUser applicationUser = new ApplicationUser()
                {
                    Id = id,
                    modifiedBy = GetCurrentUserId(),
                    updateDate = DateTime.Now
                };
                POJO model = await service.Post(applicationUser, $"{url}BlockUser", await GetToken());
                StatusMessage = $"User  {model.message} ";
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