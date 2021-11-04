using System;
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
    public class SliderController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public SliderController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Slider slider)
        {
            POJO model = new POJO();

            if (slider == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            try
            {
                model = await unitOfWork.Slider.Save(slider);
                return Ok(model);
            }
            catch (Exception ex)
            { 
                model.flag = false;
                model.message = $"APIs function is sick.  Error: {ex.ToString()}";
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
            try
            {
              Slider model = await unitOfWork.Slider.Get(id);
              return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound(ex.ToString()); 
            } 
        }

        [HttpGet("NumberOfSliders/{id}")]
        public async Task<IActionResult> NumberOfSliders(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            try
            {
               IQueryable<Slider> model = await unitOfWork.Slider.GetAll(x => x.isEnabled.Equals(true));
                return Ok(model?.Take(Convert.ToInt32(id)));
            }
            catch (Exception ex)
            {
                return NotFound(ex.ToString());
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
               IQueryable<Slider> model = await unitOfWork.Slider.GetAll();
               return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound(ex.ToString());
            }
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
           
            try
            {
                model = await unitOfWork.Slider.Delete(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = $"APIs function is sick.  Error: {ex.ToString()}";
                return Ok(model);
            }
        }
    }
}