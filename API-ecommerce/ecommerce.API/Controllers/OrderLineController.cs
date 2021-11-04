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
    public class OrderLineController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public OrderLineController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderLine orderLine)
        {
            POJO model = new POJO();

            if (orderLine == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.OrderLine.Save(orderLine);

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
            OrderLine model = await unitOfWork.OrderLine.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<OrderLine> model = await unitOfWork.OrderLine.GetAll(x => x.isDeleted.Equals(false));
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
            var orderline = await unitOfWork.OrderLine.Get(id);
            var order =  unitOfWork.Order.GetAll(x => x.id.Equals(orderline.orderId)).Result?.FirstOrDefault();
            model = await unitOfWork.OrderLine.Delete(id);
            //Update Order total after delete line
           await unitOfWork.Order.Save(new Order() 
            { 
               id = orderline.orderId,
               total = order.total - (orderline.price * orderline.qty)
            }); 

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }

        //Get Order Lines by Order Id in AZ to reduse the product ty
        [HttpGet("GetOrderLines/{id}")]
        public async Task<IActionResult> GetOrderLines(string id)
        {
            try
            {
                if (String.IsNullOrEmpty(id))
                    return NotFound();

                return Ok(await unitOfWork.OrderLine.GetAll(x => x.orderId.Equals(id)));
                 
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }
    }
}