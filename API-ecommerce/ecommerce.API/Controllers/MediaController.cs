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
    public class MediaController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public MediaController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Media media)
        {
            POJO model = new POJO();

            if (media == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

             model = await unitOfWork.Media.Save(media);

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
            Media model = await unitOfWork.Media.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Media> model = await unitOfWork.Media.GetAll(x => x.isDeleted.Equals(false));
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
             model = await unitOfWork.Media.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }

        [HttpPost("UpdateIsDeleted")]
        public async Task<IActionResult> UpdateIsDeleted([FromBody] Media media)
        {
            POJO model = new POJO();

            if (media == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Media.UpdateIsDelete(media);

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