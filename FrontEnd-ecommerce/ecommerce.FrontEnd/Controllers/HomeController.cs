using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ecommerce.FrontEnd.Models;
using ecommerce.Data;
using UtilityFunctions.Data;
using System.Linq;
using Libraries.ecommerce.Services.Services;
using Libraries.ecommerce.Services.Repositories;
using Microsoft.AspNetCore.Authentication;
using Libraries.ecommerce.GoogleReCaptcha.Services;
using Libraries.ecommerce.GoogleReCaptcha.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace ecommerce.FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IService<Setting> service;
        private readonly string url = "/api/setting/";
        private readonly EmailSenderService serviceEmailSender;
        //FAQ
        private readonly IService<FAQ> serviceFAQ;
        private readonly string urlFAQ = "/api/faq/";
        //Google Recaptcha
        private readonly IGoogleReCaptchaService googleReCaptchaService;
        //Pages
        private readonly IService<Page> servicePage;
        private readonly string urlPage = "/api/page/";
        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public HomeController(ILogger<HomeController> logger, IService<Setting> _service,
            EmailSenderService _serviceEmailSender, IService<FAQ> _serviceFAQ,
            IGoogleReCaptchaService _googleReCaptchaService, IService<Page> _servicePage)
        {
            _logger = logger;
            service = _service;
            serviceEmailSender = _serviceEmailSender;
            serviceFAQ = _serviceFAQ;
            googleReCaptchaService = _googleReCaptchaService;
            servicePage = _servicePage;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var settings = await service.GetAll(url, await GetToken());
                return View(settings?.FirstOrDefault());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Home Page");
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Medi_Access")))
            {
                HttpContext.Session.SetString("_Medi_Access", await service.GetAccessToken());
            }
            return HttpContext.Session.GetString("_Medi_Access");             
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }

        public async Task<IActionResult> FAQ()
        {
            IQueryable<FAQ> fAQ = null;
            try
            {
                fAQ = await serviceFAQ.GetAll(urlFAQ, await GetToken());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "un-registerd user.");
                return RedirectToAction("Error", "Home");
            }
            return View(fAQ);
        }

        public async Task<IActionResult> Page(string id)
        {
            Page page = new Data.Page();
            try
            {
                if (String.IsNullOrEmpty(id))
                {
                    _logger.LogError("Home Controller Empty Page Id");
                    return RedirectToAction("Error", "Home");
                }

                page = await servicePage.Get(id, urlPage, await GetToken());

                if (User.Identity.IsAuthenticated && page.isPrivate)
                {
                    return View(page);
                }
                else if (!page.isPrivate)
                {
                    return View(page);
                }
                else
                {
                    _logger.LogError("Home Controller Page, un-registerd user.");
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "un-registerd user.");
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendFeedback(MVFeedBackForm mVFeedBackForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool verified = await googleReCaptchaService.GoogleReCaptchaVerification(new GoogleReCaptchaRequest() { response = mVFeedBackForm.token });
                    if (!verified)
                    {
                        StatusMessage = $"Your Connections is not secure.";
                        return RedirectToAction("ContactUs", "Home");
                    }

                    var settings = await service.GetAll(url, await GetToken());
                    var setting = settings.FirstOrDefault();
                    await serviceEmailSender.SendEmail
                                 (
                                 new EmailSenderRequest()
                                 {
                                     senderName = mVFeedBackForm.name,
                                     from = setting.email,
                                     subject = $"Feedback Form, Subject: {mVFeedBackForm.subject}",
                                     to = setting.email,
                                     cc = mVFeedBackForm.email,
                                     receiverName = setting.websiteName,
                                     plainTextContent = mVFeedBackForm.message,
                                     htmlContent = mVFeedBackForm.message
                                 });
                    StatusMessage = $"Thank you for your email.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Home Controller Email user, Form Feedback");
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("ContactUs", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SendErrorEmail(ErrorViewModel errorView)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool verified = await googleReCaptchaService.GoogleReCaptchaVerification(new GoogleReCaptchaRequest() { response = errorView.Token });
                    if (!verified)
                    {
                        StatusMessage = $"You are not human.";
                        return RedirectToAction("Error", "Home", errorView);
                    }

                    if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Medi_Access")))
                    {

                        HttpContext.Session.SetString("_Medi_Access", await GetToken());
                    }

                    var token = HttpContext.Session.GetString("_Medi_Access");

                    var settings = await service.GetAll(url, token);
                    var setting = settings?.FirstOrDefault();
                    await serviceEmailSender.SendEmail
                                 (
                                 new EmailSenderRequest()
                                 {
                                     senderName = "Error Front End.",
                                     from = setting.email,
                                     subject = $"Error Front End",
                                     to = setting.helpDeskEmail,
                                     cc = setting.email,
                                     receiverName = setting.websiteName,
                                     plainTextContent = errorView.RequestId,
                                     htmlContent = errorView.RequestId
                                 });
                    StatusMessage = $"Thank you for reporting this glitch.";
                }
                catch
                {
                    StatusMessage = $"Sorry, Its glitch in the error page :(";
                    return RedirectToAction("Error", "Home", errorView);
                }
            }
            return RedirectToAction("Error", "Home", errorView);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Home/Index"
            }, "oidc");
        }

        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/Home/Index" },
                "ExampleCookies", "oidc");
        }
    }
}
