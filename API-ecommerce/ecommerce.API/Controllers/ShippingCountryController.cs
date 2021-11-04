using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShippingCountryController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public ShippingCountryController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ShippingCountry  shippingCountry)
        {
            POJO model = new POJO();

            if (shippingCountry == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.ShippingCountry.Save(shippingCountry);

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
            ShippingCountry model = await unitOfWork.ShippingCountry.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("GetByCountry/{id}")]
        public async Task<IActionResult> GetByCountry(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = await unitOfWork.ShippingCountry.GetAll(x => x.country.Equals(id));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model?.FirstOrDefault());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<ShippingCountry> model = await unitOfWork.ShippingCountry.GetAll();
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

            model = await unitOfWork.ShippingCountry.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        } 
    }
}
