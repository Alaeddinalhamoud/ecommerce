using System.Linq;
using System.Net.Http;
using System.Threading.Tasks; 
using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IService<User> service;
        private readonly IService<ApplicationUser> serviceApplicationUser;
        private readonly IService<ChangePasswordViewModel> serviceChangePasswordViewModel;
        private readonly string url = "/api/User/";
        public IConfiguration Configuration { get; }

        public UserController(IConfiguration configuration, IService<User> _service,
            IService<ApplicationUser> _serviceApplicationUser,
            IService<ChangePasswordViewModel> _serviceChangePasswordViewModel)
        {
            Configuration = configuration;
            service = _service;
            serviceApplicationUser = _serviceApplicationUser;
            serviceChangePasswordViewModel = _serviceChangePasswordViewModel;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                IQueryable<User> users = await service.GetAll(url, await GetToken());
                return Ok(users);
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                ApplicationUser applicationUser = await serviceApplicationUser.Get(id, url, await GetToken());
                return Ok(applicationUser);
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.ToString());
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(ApplicationUser applicationUser)
        {
            POJO model = null;
            try
            {
                model = await serviceApplicationUser.Post(applicationUser, url, await GetToken());
                return Ok(model);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("BlockUser")]
        public async Task<IActionResult> BlockUser(ApplicationUser applicationUser)
        {
            POJO model = null;
            try
            {
                model = await serviceApplicationUser.Post(applicationUser, $"{url}BlockUser", await GetToken());
                return Ok(model);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            POJO model = null;
            try
            {
                model = await serviceChangePasswordViewModel.Post(changePasswordViewModel, $"{url}ChangePassword", await GetToken());
                
                return Ok(model);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        [HttpPost("SaveUserDetails")]
        public async Task<IActionResult> SaveUserDetails(ApplicationUser applicationUser)
        {
            POJO model = null;
            try
            {
                model = await serviceApplicationUser.Post(applicationUser, $"{url}SaveUserDetails", await GetToken());
                return Ok(model);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}