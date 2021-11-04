using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendorBankController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public VendorBankController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VendorBank vendorBank)
        {
            POJO model = new POJO();

            if (vendorBank == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.VendorBank.Save(vendorBank);

            if (model == null)
            {
                model.flag = false;
                model.message = "VendorApplication API function is sick.";
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
            VendorBank model = await unitOfWork.VendorBank.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<VendorBank> model = await unitOfWork.VendorBank.GetAll();
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("GetByVendorProfileId/{id}")]
        public async Task<IActionResult> GetByVendorProfileId(int id)
        {
            try
            {
                IQueryable<VendorBank> model = await unitOfWork.VendorBank.GetAll(x => x.vendorProfileId.Equals(id));
                if (model == null)
                {
                    return NotFound();
                }
                return Ok(model?.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Your bank Detials DB is sick, Error: {ex.ToString()}");
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

            model = await unitOfWork.VendorBank.Delete(id);

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
