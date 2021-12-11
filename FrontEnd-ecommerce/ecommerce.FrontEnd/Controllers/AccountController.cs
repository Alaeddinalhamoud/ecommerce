using AutoMapper;
using ecommerce.Data;
using ecommerce.Data.MVData;
using ecommerce.FrontEnd.Models;
using Libraries.ecommerce.RazorHtmlEmails.Services;
using Libraries.ecommerce.RazorHtmlEmails.Views.Emails.GenericText;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.FrontEnd.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        //Calling  services
        private readonly IService<ApplicationUser> service;
        private readonly IService<ChangePasswordViewModel> serviceChangePasswordViewModel;
        private readonly string url = "/api/User/";

        private readonly IService<IEnumerable<Order>> serviceOrder;
        private readonly string urlOrder = "/api/Order/";
        private readonly IService<IEnumerable<Address>> serviceAddresses;
        private readonly IService<Address> serviceAddress;
        private readonly string urlAddress = "/api/Address/";

        private readonly IService<ShippingCountry> serviceShippingCountry;
        private readonly string urlShippingCountry = "/api/ShippingCountry/";

        private readonly IService<VendorApplication> serviceVendorApplication;
        private readonly string urlVendorApplication = "/api/VendorApplication/";

        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";

        private readonly EmailSenderService serviceEmailSender;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;

        //FileUploader
        private readonly FileUploadService fileUploadService;

        //AutoMapper
        private readonly IMapper mapper;
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public AccountController(IService<ApplicationUser> _service, IService<Address> _serviceAddress,
            IService<ChangePasswordViewModel> _serviceChangePasswordViewModel,
            IService<IEnumerable<Order>> _serviceOrder, IService<VendorApplication> _serviceVendorApplication,
            EmailSenderService _serviceEmailSender, IService<Setting> _serviceSetting,
            IRazorViewToStringRenderer _razorViewToStringRenderer, IMapper _mapper, FileUploadService _FileUploadService,
            IService<ShippingCountry> _serviceShippingCountry, IService<IEnumerable<Address>> _serviceAddresses,
            ILogger<AccountController> logger)
        {
            service = _service;
            serviceAddress = _serviceAddress;
            serviceChangePasswordViewModel = _serviceChangePasswordViewModel;
            serviceOrder = _serviceOrder;
            serviceVendorApplication = _serviceVendorApplication;
            serviceEmailSender = _serviceEmailSender;
            serviceSetting = _serviceSetting;
            razorViewToStringRenderer = _razorViewToStringRenderer;
            mapper = _mapper;
            fileUploadService = _FileUploadService;
            serviceShippingCountry = _serviceShippingCountry;
            serviceAddresses = _serviceAddresses;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser applicationUser = null;
            try
            {
                applicationUser = await service.Get(GetCurrentUserId(), $"{url}", await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(applicationUser);
        }
        //Current User
        [HttpGet]
        public async Task<IActionResult> AccountDetails()
        {
            MVApplicationUser mVApplicationUser = null;
            try
            {
                ApplicationUser applicationUser = await service.Get(GetCurrentUserId(), $"{url}", await GetToken());
                mVApplicationUser = mapper.Map<MVApplicationUser>(applicationUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(mVApplicationUser);
        }
        [HttpPost, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AccountDetails(MVApplicationUser mVApplicationUser)
        {

            //Check same user Id in back too.
            bool isSecured = GetCurrentUserId().Equals(mVApplicationUser.Id) ? true : false;
            if (!isSecured)
            {
                _logger.LogError($"Not secured opeartion By {GetCurrentUserId()}");
                return PartialView("_Failure", "Error");
            }

            POJO model = null;
            try
            {
                ApplicationUser applicationUser = mapper.Map<ApplicationUser>(mVApplicationUser);
                model = await service.Post(applicationUser, $"{url}SaveUserDetails", await GetToken());
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Model error: {model.message}, UserId: {GetCurrentUserId()}");
                return PartialView("_Failure", "Error");
            }

            return PartialView("_Success");
        }

        //Check if the Vendor has any pending application 
        [HttpGet]
        public async Task<IActionResult> VendorApplication()
        {
            MVVendorApplication mVVendorApplication = new MVVendorApplication();
            try
            {
                var vendorApplication = await serviceVendorApplication.Get(GetCurrentUserId(), $"{urlVendorApplication}GetByUserId/", await GetToken());
                var settings = await serviceSetting.GetAll(urlSetting, await GetToken());
                var setting = settings?.FirstOrDefault();
                if (vendorApplication == null)
                {
                    mVVendorApplication.id = 0;
                    mVVendorApplication.status = Status.Open;
                    mVVendorApplication.vendorAgreementContract = setting.vendorAgreementContract;
                    return View(mVVendorApplication);
                }
                mVVendorApplication = mapper.Map<MVVendorApplication>(vendorApplication);
                mVVendorApplication.vendorAgreementContract = setting.vendorAgreementContract;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(mVVendorApplication);
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> VendorApplication(MVVendorApplication mVVendorApplication)
        {
            POJO model = null;
            if (ModelState.IsValid)
            {
                try
                {
                    VendorApplication vendorApplication = new VendorApplication();
                    vendorApplication.id = mVVendorApplication.id;
                    vendorApplication.fullName = mVVendorApplication.fullName;
                    vendorApplication.companyName = mVVendorApplication.companyName;
                    vendorApplication.companyVAT = mVVendorApplication.companyVAT;
                    vendorApplication.workEmail = mVVendorApplication.workEmail;
                    vendorApplication.tel1 = mVVendorApplication.tel1;
                    vendorApplication.tel2 = mVVendorApplication.tel2;
                    vendorApplication.crNumber = mVVendorApplication.crNumber;
                    vendorApplication.ownerIdNumber = mVVendorApplication.ownerIdNumber;
                    vendorApplication.companyAddress = mVVendorApplication.companyAddress;
                    vendorApplication.bankName = mVVendorApplication.bankName;
                    vendorApplication.bankAddress = mVVendorApplication.bankAddress;
                    vendorApplication.account = mVVendorApplication.account;
                    vendorApplication.swiftCode = mVVendorApplication.swiftCode;
                    vendorApplication.iBAN = mVVendorApplication.iBAN;
                    vendorApplication.createDate = DateTime.Now;
                    vendorApplication.createdBy = GetCurrentUserId();
                    vendorApplication.modifiedBy = GetCurrentUserId();
                    vendorApplication.status = Status.Open;

                    model = await serviceVendorApplication.Post(vendorApplication, urlVendorApplication, await GetToken());

                    //Upload the files
                    if (mVVendorApplication.fileToUpload != null)
                    {
                        await FileUploder(mVVendorApplication, model);
                    }

                    //Send Email confiramtion
                    await SendEmail("Thank you, We have received your request to change your current account to Seller account.", vendorApplication.workEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Model Error: {model.message}, UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                StatusMessage = $"Vendor Application Has been sent.";
                return View();
            }
            return View(mVVendorApplication);
        }

        private async Task FileUploder(MVVendorApplication mVVendorApplication, POJO model)
        {
            foreach (var formFile in mVVendorApplication.fileToUpload)
            {
                if (formFile.Length > 0 && formFile.ContentType.Equals("application/pdf"))
                {

                    FileUploaderRequest fileUploaderRequest = new FileUploaderRequest();
                    fileUploaderRequest.contentType = formFile.ContentType;
                    fileUploaderRequest.fileExtention = Path.GetExtension(formFile.FileName);
                    fileUploaderRequest.userId = GetCurrentUserId();
                    fileUploaderRequest.id = Convert.ToInt32(model.id);
                    fileUploaderRequest.sourceController = "VendorApplication";

                    using (var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);
                        fileUploaderRequest.content = Convert.ToBase64String(memoryStream.ToArray());
                    }

                    await fileUploadService.FileUpload(fileUploaderRequest);
                }
            }
        }

        //Current User
        [HttpGet]
        public async Task<IActionResult> EditAddress(string id)
        {
            Address address = null;
            try
            {
                address = await serviceAddress.Get(id, urlAddress, await GetToken());
                ViewBag.ShippingCountries = await serviceShippingCountry.GetAll(urlShippingCountry, await GetToken());
                if (address == null)
                {
                    return View("SaveAddress");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View("SaveAddress", address);
        }

        [HttpGet]
        public async Task<IActionResult> SaveAddress()
        {
            try
            {
                ViewBag.ShippingCountries = await serviceShippingCountry.GetAll(urlShippingCountry, await GetToken());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View();
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SaveAddress(Address address)
        {
            //Check same user Id in back too.
            if (!address.id.Equals(0))
            {
                bool isSecured = GetCurrentUserId().Equals(address.createdBy) ? true : false;
                if (!isSecured)
                {
                    _logger.LogError($"Not secured opeartion By {GetCurrentUserId()}", GetCurrentUserId());
                    return RedirectToAction("Error", "Home");
                }
            }

            POJO model = null;
            try
            {

                //set the creator and modifier
                address.modifiedBy = GetCurrentUserId();
                address.createdBy = GetCurrentUserId();
                ViewBag.ShippingCountries = await serviceShippingCountry.GetAll(urlShippingCountry, await GetToken());
                model = await serviceAddress.Post(address, urlAddress, await GetToken());
                return RedirectToAction(nameof(MyAddresses));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                changePasswordViewModel.UserId = GetCurrentUserId();

                POJO model = null;
                try
                {
                    model = await serviceChangePasswordViewModel.Post(changePasswordViewModel, $"{url}ChangePassword", await GetToken());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    ModelState.AddModelError("Error", ex.ToString());
                    return PartialView("_Failure", model.message);
                }
                if (!model.flag)
                {
                    ModelState.AddModelError("Error", model.message.ToString());
                    return PartialView("_Failure", model.message);
                }
                return PartialView("_Success");
            }
            var query = from state in ModelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;
            string errors = "";
            foreach (var item in query)
            {
                errors = errors + item;
            }
            return PartialView("_Failure", errors.ToString()); ;
        }

        [AllowAnonymous]
        public IActionResult AccessDenied(string ReturnUrl)
        {
            if (GetCurrentUserId() != null)
            {
                _logger.LogError($"Access Denied Event", $" {GetCurrentUserId()}, Access Denied Page, ReturnUrl {ReturnUrl} ");
            }
            else
            {
                _logger.LogError($"Access Denied Event", $" Visitor, Access Denied Page, ReturnUrl {ReturnUrl} ");
            }
            return View();
        }

        public async Task<IActionResult> MyOrders()
        {
            IEnumerable<Order> order = null;
            try
            {
                order = await serviceOrder.Get(GetCurrentUserId(), $"{urlOrder}GetOrderByUserId/", await GetToken());

                if (order == null)
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(order);
        }

        public async Task<IActionResult> MyAddresses()
        {
            IEnumerable<Address> addresses = null;
            try
            {
                addresses = await serviceAddresses.Get(GetCurrentUserId(), $"{urlAddress}GetAddressesByUserId/", await GetToken());

                if (addresses == null)
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(addresses);
        }

        public async Task<IActionResult> DeleteAddress(int id)
        {
            POJO model = new POJO();
            try
            {

                if (id != 0)
                {
                    model = await serviceAddresses.Delete(id, $"{urlAddress}", await GetToken());
                }
                return RedirectToAction(nameof(MyAddresses));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task SendEmail(string body, string email)
        {
            var settings = await serviceSetting.GetAll(urlSetting, await GetToken());
            var setting = settings?.FirstOrDefault();
            string emailBody = await razorViewToStringRenderer.RenderViewToStringAsync(
                "/Views/Emails/GenericText/GenericTextEmail.cshtml",
                new GenericTextEmailViewModel(body));

            await serviceEmailSender.SendEmail(
                           new EmailSenderRequest()
                           {
                               senderName = setting.websiteName,
                               from = setting.email,
                               subject = $"Seller Application",
                               to = email,
                               plainTextContent = emailBody,
                               htmlContent = emailBody
                           });
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult Is18YearsOld(DateTime dOB)
        {
            // Save today's date.
            var today = DateTime.Today;

            // Calculate the age.
            var age = today.Year - dOB.Year;

            if (age > 18)
            {
                return Json(true);
            }
            else
            {
                return Json($"You have to be 18 years old.");
            }
        }

        public string GetCurrentUserId()
        {
            return User?.FindFirst(c => c.Type == "sub")?.Value;
        }
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}