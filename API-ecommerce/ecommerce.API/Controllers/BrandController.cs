using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public BrandController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Brand brand)
        {
            POJO model = new POJO();

            if (brand == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

             model = await unitOfWork.Brand.Save(brand);

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
            Brand model = await unitOfWork.Brand.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Brand> model = await unitOfWork.Brand.GetAll(x => x.isDeleted.Equals(false));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        } 

        [HttpGet("GetDeletedBrands")]
        public async Task<IActionResult> GetDeletedBrands()
        {
            IQueryable<Brand> model = await unitOfWork.Brand.GetAll(x => x.isDeleted.Equals(true));
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

             model = await unitOfWork.Brand.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpPost("UpdateIsDeleted")]
        public async Task<IActionResult> UpdateIsDeleted([FromBody] Brand brand)
        {
            POJO model = new POJO();

            if (brand == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Brand.UpdateIsDelete(brand);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }
    }
}