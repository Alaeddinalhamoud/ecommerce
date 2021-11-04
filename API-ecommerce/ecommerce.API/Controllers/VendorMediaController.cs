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
    public class VendorMediaController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public VendorMediaController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VendorMedia media)
        {
            POJO model = new POJO();

            if (media == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

             model = await unitOfWork.VendorMedia.Save(media);

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
            VendorMedia model = await unitOfWork.VendorMedia.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<VendorMedia> model = await unitOfWork.VendorMedia.GetAll();
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
             model = await unitOfWork.VendorMedia.Delete(id);

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