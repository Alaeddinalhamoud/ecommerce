using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingOrderController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public TrackingOrderController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrackingOrder trackingOrder)
        {
            POJO model = new POJO(); 
            try
            {
                var trackingOrderId = trackingOrder.id;
                if (trackingOrder == null)
                {
                    model.flag = false;
                    model.message = "Empty request.";
                    return Ok(model);
                }
                model = await unitOfWork.TrackingOrder.Save(trackingOrder);

                //Update the order status, Call update Order only if not new tracking, (We have already created order with ordered status)
                if (trackingOrderId != 0)
                {
                    model = await unitOfWork.Order.Save(new Order()
                    {
                        id = trackingOrder.orderId,
                        updateDate = DateTime.Now,
                        modifiedBy = trackingOrder.userId,
                        trackingStatus = trackingOrder.trackingStatus
                    });
                }
                if (model == null)
                {
                    model.flag = false;
                    model.message = "APIs function is sick.";
                    return Ok(model);
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = ex.ToString();
                return Ok(model);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            TrackingOrder model = await unitOfWork.TrackingOrder.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("PendingShipments")]
        public async Task<IActionResult> GetPendingShipments()
        {
            IQueryable<TrackingOrder> model = await unitOfWork.TrackingOrder.GetAll(x => x.trackingStatus != TrackingStatus.Delivered);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("ShippedOrders")]
        public async Task<IActionResult> GetShippedOrders()
        {
            IQueryable<TrackingOrder> model = await unitOfWork.TrackingOrder.GetAll(x => x.trackingStatus.Equals(TrackingStatus.Delivered));
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

            model = await unitOfWork.TrackingOrder.Delete(id);

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