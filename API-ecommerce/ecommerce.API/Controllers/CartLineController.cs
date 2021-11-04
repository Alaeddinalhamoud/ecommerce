using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartLineController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public CartLineController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CartLine cartLine)
        {
            POJO model = new POJO();

            if (cartLine == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

             model = await unitOfWork.CartLine.Save(cartLine);
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
            CartLine model = await unitOfWork.CartLine.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<CartLine> model = await unitOfWork.CartLine.GetAll();
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }
        //Will delete the cartline and update the cart total too.
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
            //Delete from cartline
             model = await unitOfWork.CartLine.Delete(id);
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