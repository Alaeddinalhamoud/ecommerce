using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IService<ApplicationUser> serviceApplicationUser;
        private readonly string url = "/api/User/";
        public AlertController(IUnitOfWork _UnitOfWork, IService<ApplicationUser> _serviceApplicationUser)
        {
            unitOfWork = _UnitOfWork;
            serviceApplicationUser = _serviceApplicationUser;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Alert alert)
        {
            POJO model = new POJO();

            if (alert == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Alert.Save(alert);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Alert model = await unitOfWork.Alert.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Alert> model = await unitOfWork.Alert.GetAll();
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        } 

        [HttpGet("PublicAlert")]
        public async Task<IActionResult> PublicAlert()
        {
            IQueryable<Alert> model = await unitOfWork.Alert.GetAll(x =>x.isEnabled.Equals(true));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            POJO model = new POJO();

            if (id == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Alert.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpGet("UserProfileAlert/{id}")]
        public async Task<IActionResult> UserProfileAlert(string id)
        {
            Alert model = new Alert(); 
            try
            {
                //Check the user address and the user profile if not complated.
                var address = await unitOfWork.Address.GetAll(x => x.createdBy.Equals(id));
                var userApplication = await serviceApplicationUser.Get(id, url, await GetToken());

                model.isEnabled = false;
                model.body = "Customer is Fine, He can make order.";

                if (!address.Any() || String.IsNullOrEmpty(userApplication.PhoneNumber))
                {
                    var addressText = !address.Any() ? "Address" : "";
                    var phoneNumberText = String.IsNullOrEmpty(userApplication.PhoneNumber) ? "Phone Number" : "";
                    model.isEnabled = true;
                    model.title = $"Hi {userApplication.name},";
                    model.body = $"To be able to make an order you have to complete your Account, {addressText} & {phoneNumberText}";
                }

                return Ok(model);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
