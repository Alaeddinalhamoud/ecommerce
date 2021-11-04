using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
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
    public class VendorProfileController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public VendorProfileController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VendorProfile vendorProfile)
        {
            POJO model = new POJO();

            if (vendorProfile == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.VendorProfile.Save(vendorProfile);

            if (model == null)
            {
                model.flag = false;
                model.message = "Vendor Profile API function is sick.";
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
            VendorProfile model = await unitOfWork.VendorProfile.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<VendorProfile> model = await unitOfWork.VendorProfile.GetAll();
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        } 

        [HttpGet("GetByVendorId/{id}")]
        public async Task<IActionResult> GetByVendorId(string id)
        {
            try
            {
                IQueryable<VendorProfile> model = await unitOfWork.VendorProfile.GetAll(x => x.vendorId.Equals(id));
                if (model == null)
                {
                    return NotFound();
                }
                return Ok(model?.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Your Vendor profile DB is sick, Error: {ex.ToString()}");
            }
        }

        [HttpGet("GetVendorProfileDetails/{id}")]
        public async Task<IActionResult> GetVendorProfileDetails(string id)
        { 
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                VendorProfileDetails vendorProfileDetails = new VendorProfileDetails();
                VendorProfile vendorProfile = await unitOfWork.VendorProfile.Get(Convert.ToInt32(id)); 

                if (vendorProfile == null)
                {
                    return NotFound();
                }

                var vendorBank = await unitOfWork.VendorBank.GetAll(x => x.vendorProfileId.Equals(Convert.ToInt32(id)));
                var vendorMedia = await unitOfWork.VendorMedia.GetAll(x => x.vendorId.Equals(vendorProfile.vendorId));

                if (vendorProfile != null)
                {
                    vendorProfileDetails.VendorProfileId = vendorProfile.id;
                    vendorProfileDetails.companyName = vendorProfile.companyName;
                    vendorProfileDetails.companyVAT = vendorProfile.companyVAT;
                    vendorProfileDetails.workEmail = vendorProfile.workEmail;
                    vendorProfileDetails.tel1 = vendorProfile.tel1;
                    vendorProfileDetails.tel2 = vendorProfile.tel2;
                    vendorProfileDetails.crNumber = vendorProfile.crNumber;
                    vendorProfileDetails.ownerIdNumber = vendorProfile.ownerIdNumber;
                    vendorProfileDetails.companyAddress = vendorProfile.companyAddress;
                    vendorProfileDetails.companyAddress = vendorProfile.companyAddress;
                    vendorProfileDetails.status = vendorProfile.status;
                }

                if(vendorBank.FirstOrDefault() != null)
                {
                    var model = vendorBank.FirstOrDefault();
                    vendorProfileDetails.bankId = model.id;
                    vendorProfileDetails.bankName = model.bankName;
                    vendorProfileDetails.bankAddress = model.bankAddress;
                    vendorProfileDetails.account = model.account;
                    vendorProfileDetails.swiftCode = model.swiftCode;
                    vendorProfileDetails.iBAN = model.iBAN;
                }
                
                if(vendorMedia != null)
                {
                    vendorProfileDetails.vendorMedias = vendorMedia;
                }

                
                return Ok(vendorProfileDetails);
            }
            catch (Exception ex)
            {
                return BadRequest($"Your Vendor profile DB is sick, Error: {ex.ToString()}");
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

            model = await unitOfWork.VendorProfile.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
