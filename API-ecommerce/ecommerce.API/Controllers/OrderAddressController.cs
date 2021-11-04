using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAddressController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public OrderAddressController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderAddress orderAddress)
        {
            POJO model = new POJO();

            if (orderAddress == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }
             model = await unitOfWork.OrderAddress.Save(orderAddress);

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
            OrderAddress model = await unitOfWork.OrderAddress.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        } 
       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<OrderAddress> model =await unitOfWork.OrderAddress.GetAll();
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

             model = await unitOfWork.OrderAddress.Delete(id);

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