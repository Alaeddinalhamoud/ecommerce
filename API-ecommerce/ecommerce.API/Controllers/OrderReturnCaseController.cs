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
    public class OrderReturnCaseController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public OrderReturnCaseController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderReturnCase orderReturnCase)
        {
            POJO model = new POJO();

            if (orderReturnCase == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.OrderReturnCase.Save(orderReturnCase);

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
            OrderReturnCase model = await unitOfWork.OrderReturnCase.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<OrderReturnCase> model = await unitOfWork.OrderReturnCase.GetAll();
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

            model = await unitOfWork.OrderReturnCase.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpGet("GetOrderReturnCaseByOrder/{id}")]
        public async Task<IActionResult> GetOrderReturnCaseByOrder(string id)
        {
            IQueryable<OrderReturnCase> model = await unitOfWork.OrderReturnCase.GetAll(x => x.orderId.Equals(id) && (x.status.Equals(Status.Pending) || x.status.Equals(Status.Returned)));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model?.FirstOrDefault());
        }

    }
}
