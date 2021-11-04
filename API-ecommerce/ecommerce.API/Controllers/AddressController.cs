using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public AddressController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Address address)
        {
            POJO model = new POJO();

            if (address == null) 
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }
             model = await unitOfWork.Address.Save(address);

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
            Address model = await unitOfWork.Address.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("GetByUserId/{id}")]
        public async Task<IActionResult> GetByUserId(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            IQueryable<Address> model = await unitOfWork.Address.GetAll(x => x.modifiedBy.Equals(id) && x.isDeleted.Equals(false));
            if (model.Count().Equals(0))
            {
                Address address = new Address()
                {
                    id= 0
                };
                return Ok(address);
            }
            //Curently we are working on one addrsss.
            return Ok(model?.FirstOrDefault()); ;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Address> model =await unitOfWork.Address.GetAll(x => x.isDeleted.Equals(false));
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

             model = await unitOfWork.Address.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpPost("UpdateIsDeleted")]
        public async Task<IActionResult> UpdateIsDeleted([FromBody] Address address)
        {
            POJO model = new POJO();

            if (address == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Address.UpdateIsDelete(address);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpGet("GetAddressesByUserId/{id}")]
        public async Task<IActionResult> GetAddressesByUserId(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            IEnumerable<Address> model = await unitOfWork.Address.GetAll(x => x.createdBy.Equals(id) && x.isDeleted.Equals(false));
            if (model is null && model.Count() <= 0)
            {
                return NotFound($"NO address for this user {id}");
            }
            return Ok(model.OrderByDescending(x => x.createDate));
        }

    }
}