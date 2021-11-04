using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using ecommerce.Services;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendorApplicationController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IService<ApplicationUser> serviceApplicationUser;
        private readonly string url = "/api/User/";
        public VendorApplicationController(IUnitOfWork _UnitOfWork, IService<ApplicationUser> _serviceApplicationUser)
        {
            unitOfWork = _UnitOfWork;
            serviceApplicationUser = _serviceApplicationUser;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VendorApplication vendorApplication)
        {
            POJO model = new POJO();

            if (vendorApplication == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.VendorApplication.Save(vendorApplication);

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
            VendorApplication model = await unitOfWork.VendorApplication.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<VendorApplication> model = await unitOfWork.VendorApplication.GetAll();
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("GetApprovedFormApplication")]
        public async Task<IActionResult> GetApprovedFormApplication()
        {
            try
            {
                IQueryable<VendorApplication> model = await unitOfWork.VendorApplication.GetAll(x => x.status.Equals(Status.Closed));
                if (model == null)
                {
                    return NotFound();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"Your FormApp DB is sick, Error: {ex.ToString()}");
            }
        }


        [HttpGet("GetPendingFormApplication")]
        public async Task<IActionResult> GetPendingFormApplication()
        {
            try
            {
                IQueryable<VendorApplication> model = await unitOfWork.VendorApplication.GetAll(x => x.status.Equals(Status.Open)
                                                                                                 || x.status.Equals(Status.Pending));
                if (model == null)
                {
                    return NotFound();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"Your FormApp DB is sick, Error: {ex.ToString()}");
            } 
        }

        [HttpGet("GetByUserId/{id}")]
        public async Task<IActionResult> GetByUserId(string id)
        {
            try
            {
                IQueryable<VendorApplication> model = await unitOfWork.VendorApplication.GetAll(x => x.createdBy.Equals(id));
                if (model == null)
                {
                    return NotFound();
                }
                return Ok(model?.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Your FormApp DB is sick, Error: {ex.ToString()}");
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

            model = await unitOfWork.VendorApplication.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        }

        //Approve App and Create Profile and bank
        [HttpPost("ApproveVendorApplication")]
        public async Task<IActionResult> ApproveVendorApplication([FromBody] VendorApplication vendorApp)
        {
            try
            {
                POJO model = new POJO();

                if (vendorApp == null)
                {
                    model.flag = false;
                    model.message = "Empty request.";
                    return Ok(model);
                }

                model = await unitOfWork.VendorApplication.Save(vendorApp);

                var vendorApplication = await unitOfWork.VendorApplication.Get(vendorApp.id);
                if (model == null)
                {
                    model.flag = false;
                    model.message = "VendorApplication API function is sick.";
                    return Ok(model);
                }

                if (vendorApplication != null)
                {
                    //2- Create Vendor Profile.
                    var vendorProfile = new VendorProfile()
                    {
                        vendorId = vendorApplication.createdBy,
                        companyName = vendorApplication.companyName,
                        companyVAT = vendorApplication.companyVAT,
                        workEmail = vendorApplication.workEmail,
                        tel1 = vendorApplication.tel1,
                        tel2 = vendorApplication.tel2,
                        crNumber = vendorApplication.crNumber,
                        ownerIdNumber = vendorApplication.ownerIdNumber,
                        companyAddress = vendorApplication.companyAddress,
                        status = Status.Open,
                        //We have sent it form the dash user who accept the app he will be the creator
                        createdBy = vendorApplication.modifiedBy,
                        createDate = DateTime.Now
                    };

                    model = await unitOfWork.VendorProfile.Save(vendorProfile);

                    //3- Bank Details
                    var vendorBank = new VendorBank()
                    {
                        vendorProfileId = vendorProfile.id,
                        bankName = vendorApplication.bankName,
                        bankAddress = vendorApplication.bankAddress,
                        account = vendorApplication.account,
                        swiftCode = vendorApplication.swiftCode,
                        iBAN = vendorApplication.iBAN,
                        createdBy = vendorApplication.modifiedBy,
                        createDate = DateTime.Now
                    };
                    model = await unitOfWork.VendorBank.Save(vendorBank);
                    //4 Update the user Roll to vendor
                    ApplicationUser applicationUser = new ApplicationUser()
                    {
                        Id = vendorApplication.createdBy,
                        isBlocked = false,
                        isVendor = true, 
                        modifiedBy = vendorApplication.modifiedBy,
                        updateDate = DateTime.Now
                    };
                    model = await serviceApplicationUser.Post(applicationUser, url, await GetToken());
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"Your FormApp DB is sick, Error: {ex.ToString()}");
            }
        }

        [HttpGet("GetVendorApplicationDetails/{id}")]
        public async Task<IActionResult> GetVendorApplicationDetails(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                VendorApplicationDetails vendorApplicationDetails = new VendorApplicationDetails();
                VendorApplication vendorApplication = await unitOfWork.VendorApplication.Get(Convert.ToInt32(id));

                if (vendorApplication == null)
                {
                    return NotFound();
                }
                 
                var vendorMedia = await unitOfWork.VendorMedia.GetAll(x => x.vendorId.Equals(vendorApplication.createdBy));                 
                    vendorApplicationDetails.id = vendorApplication.id;
                    vendorApplicationDetails.fullName = vendorApplication.fullName;
                    vendorApplicationDetails.companyName = vendorApplication.companyName;
                    vendorApplicationDetails.companyVAT = vendorApplication.companyVAT;
                    vendorApplicationDetails.workEmail = vendorApplication.workEmail;
                    vendorApplicationDetails.tel1 = vendorApplication.tel1;
                    vendorApplicationDetails.tel2 = vendorApplication.tel2;
                    vendorApplicationDetails.crNumber = vendorApplication.crNumber;
                    vendorApplicationDetails.ownerIdNumber = vendorApplication.ownerIdNumber;
                    vendorApplicationDetails.companyAddress = vendorApplication.companyAddress;
                    vendorApplicationDetails.companyAddress = vendorApplication.companyAddress;
                    vendorApplicationDetails.status = vendorApplication.status;
                    vendorApplicationDetails.bankName = vendorApplication.bankName;
                    vendorApplicationDetails.bankAddress = vendorApplication.bankAddress;
                    vendorApplicationDetails.account = vendorApplication.account;
                    vendorApplicationDetails.swiftCode = vendorApplication.swiftCode;
                    vendorApplicationDetails.iBAN = vendorApplication.iBAN;
                if (vendorMedia != null)
                {
                    vendorApplicationDetails.vendorMedias = vendorMedia;
                }


                return Ok(vendorApplicationDetails);
            }
            catch (Exception ex)
            {
                return BadRequest($"Your Vendor Application DB is sick, Error: {ex.ToString()}");
            }
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
