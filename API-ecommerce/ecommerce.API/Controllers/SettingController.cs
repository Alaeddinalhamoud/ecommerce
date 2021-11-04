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
    [Authorize]
    public class SettingController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public SettingController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Setting setting)
        {
            POJO model = new POJO();

            if (setting == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            try
            {
                model = await unitOfWork.Setting.Save(setting);
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
                //This methods will return only one line.
                var  models = await unitOfWork.Setting.GetAll();
                Setting setting = models?.FirstOrDefault();
                return Ok(setting);
            }
            catch (Exception ex)
            {
                return NotFound(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Setting> model = await unitOfWork.Setting.GetAll();
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }


    }
}