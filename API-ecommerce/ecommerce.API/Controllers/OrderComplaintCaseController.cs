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
    public class OrderComplaintCaseController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public OrderComplaintCaseController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderComplaintCase orderComplaintCase)
        {
            POJO model = new POJO();

            if (orderComplaintCase == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.OrderComplaintCase.Save(orderComplaintCase);

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
            OrderComplaintCase model = await unitOfWork.OrderComplaintCase.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<OrderComplaintCase> model = await unitOfWork.OrderComplaintCase.GetAll();
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

            model = await unitOfWork.OrderComplaintCase.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpGet("GetOrderComplaintCaseByOrder/{id}")]
        public async Task<IActionResult> GetOrderComplaintCaseByOrder(string id)
        {
            IQueryable<OrderComplaintCase> model = await unitOfWork.OrderComplaintCase.GetAll(x => x.orderId.Equals(id) && (x.status.Equals(Status.Pending) || x.status.Equals(Status.Returned)));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model?.FirstOrDefault());
        }

    }
}
