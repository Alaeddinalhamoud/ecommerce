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
    public class ProductTypeController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public ProductTypeController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductType productType)
        {
            POJO model = new POJO();

            if (productType == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.ProductType.Save(productType);

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
            ProductType model = await unitOfWork.ProductType.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<ProductType> model = await unitOfWork.ProductType.GetAll(x => x.isDeleted.Equals(false));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        
        [HttpGet("GetDeletedProductTypes")]
        public async Task<IActionResult> GetDeletedProductTypes()
        {
            IQueryable<ProductType> model = await unitOfWork.ProductType.GetAll(x => x.isDeleted.Equals(true));
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

            model = await unitOfWork.ProductType.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }

        [HttpPost("UpdateIsDeleted")]
        public async Task<IActionResult> UpdateIsDeleted([FromBody] ProductType productType)
        {
            POJO model = new POJO();

            if (productType == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.ProductType.UpdateIsDelete(productType);

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