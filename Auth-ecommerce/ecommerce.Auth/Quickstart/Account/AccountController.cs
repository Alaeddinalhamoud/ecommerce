// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using ecommerce.Auth.Quickstart.Models;
using ecommerce.Data;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Libraries.ecommerce.GoogleReCaptcha.Models;
using Libraries.ecommerce.GoogleReCaptcha.Services;
using Libraries.ecommerce.RazorHtmlEmails.Services;
using Libraries.ecommerce.RazorHtmlEmails.Views.Emails.ConfirmAccount;
using Libraries.ecommerce.RazorHtmlEmails.Views.Emails.ResetPassword;
using Libraries.ecommerce.Services.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace IdentityServer4.Quickstart.UI
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    //[SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly EmailSenderService _emailSenderService;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        //Google Recaptcha
        private readonly IGoogleReCaptchaService googleReCaptchaService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            EmailSenderService emailSenderService,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            IGoogleReCaptchaService _googleReCaptchaService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _emailSenderService = emailSenderService;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            googleReCaptchaService = _googleReCaptchaService;
        }


        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            bool verified = await googleReCaptchaService.GoogleReCaptchaVerification(new GoogleReCaptchaRequest() { response = model.Token });
            if (!verified)
            {
                ModelState.AddModelError(string.Empty, "Your Connection is not secure. Please, Try again");
                var vM = await BuildLoginViewModelAsync(model);
                return View(vM);
            }
                // check if we are in the context of an authorization request
                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);

                if (result.IsLockedOut)
                {
                    return LocalRedirect("/Account/IsLockedOut");
                }
                if (result.IsNotAllowed)
                {
                    return LocalRedirect("/Account/IsNotAllowed");
                }

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user.isBlocked)
                    {
                        await _signInManager.SignOutAsync();
                        await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "This user is disabled, Plesae contact the help desk."));
                        ModelState.AddModelError("BlockedUser", $"{user.UserName} Username is disabled, Plesae contact the help desk.");
                        return View(await BuildLoginViewModelAsync(model));
                    }
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));
                    var claims = await _userManager.GetClaimsAsync(user);
                    //Update login
                    user.lastLogin = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }


        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));


            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {           
            return View();
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }


        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = new RegisterVM { ReturnUrl = returnUrl };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            //  model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                bool verified = await googleReCaptchaService.GoogleReCaptchaVerification(new GoogleReCaptchaRequest() { response = model.Token });
                if (!verified)
                {
                    ModelState.AddModelError(string.Empty, "You are not human.");                   
                    return View();
                }

                IdentityResult result = null;
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, AccountOptions.UserAlreadyExistsErrorMessage);
                    return View();
                }

                user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    Email = model.Email,
                    name = model.FullName,
                    dOB = model.DOB,
                    PhoneNumber = model.phone,
                    createDate = DateTime.Now
                };

                result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //the defualt rigestration is Buyer
                    result = await _userManager.AddToRoleAsync(user, "Customer");
                    if (result.Succeeded)
                    {
                        try
                        {
                            result = await _userManager.AddClaimsAsync(user,
                                new Claim[]{
                                            new Claim(JwtClaimTypes.Subject, user.Id),
                                            new Claim(JwtClaimTypes.Email, user.Email),
                                            new Claim(JwtClaimTypes.GivenName, user.Email),
                                            new Claim(JwtClaimTypes.Name, user.name),
                                            new Claim(JwtClaimTypes.NickName, user.name),
                                });

                            //generate token for activtion 
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            //var callbackUrl = Url.Action(
                            //    "/Account/ConfirmEmail",
                            //    pageHandler: null,
                            //    values: new { userId = user.Id, code },
                            //    protocol: Request.Scheme);
                            var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { code, email = user.Email }, Request.Scheme);

                            //Call Activation Template
                            var confirmAccountModel = new ConfirmAccountEmailViewModel(callbackUrl);
                            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/ConfirmAccount/ConfirmAccountEmail.cshtml", confirmAccountModel);

                            EmailSenderResponse emailSenderResponse = await _emailSenderService.SendEmail
                                (
                                new EmailSenderRequest()
                                {
                                    senderName = "E-commerece",
                                    from = "info@Example.com",
                                    subject = "Email Confirmation",
                                    to = user.Email,
                                    receiverName = user.name,
                                    plainTextContent = "Please confirm your account.",
                                    htmlContent = body
                                });

                            if (!emailSenderResponse.flag)
                            {
                                ModelState.AddModelError(emailSenderResponse.flag.ToString(), emailSenderResponse.message.ToString());
                                return View();
                            }

                            model.ReturnUrl = "/Account/ActivationEmail";
                        }
                        catch (Exception ex)
                        {

                            ModelState.AddModelError("Connection error:", ex.ToString());
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(result.Errors.First().Code, result.Errors.First().Description);
                        return View();
                    }

                    return LocalRedirect(model.ReturnUrl);
                }
                else
                {
                    var resultErrors = result.Errors.Select(e => "<li>" + e.Description + "</li>");
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return View();
                }
            }

            var errors = ModelState.Keys.Select(e => "<li>" + e + "</li>");
            ModelState.AddModelError(string.Empty, string.Join("", errors));
            return View();
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string code, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }
        [AllowAnonymous]
        public IActionResult ActivationEmail()
        {
            return View();
        }

        public IActionResult IsLockedOut()
        {
            return View();
        }
        public IActionResult IsNotAllowed()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ForgetYourPassword()
        {
            return View();
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ForgetYourPassword(ForgetPasswordViewModel Input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ModelState.AddModelError("Error : ", "Account does not exist");
                    return View();
                }
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ModelState.AddModelError("Error : ", "Email is not confirmed");
                    return View();
                }
                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { code, email = user.Email }, Request.Scheme);

                var resetPasswordModel = new ResetPasswordEmailViewModel(callbackUrl);
                string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/ResetPassword/ResetPasswordEmail.cshtml", resetPasswordModel);

                EmailSenderResponse emailSenderResponse = await _emailSenderService.SendEmail(
                    new EmailSenderRequest()
                    {
                        senderName = "E-commerece",
                        from = "info@Example.com",
                        subject = "Reset Password",
                        to = user.Email,
                        receiverName = user.name,
                        plainTextContent = "Reset Password.",
                        htmlContent = body
                    });

                if (!emailSenderResponse.flag)
                {
                    ModelState.AddModelError(emailSenderResponse.flag.ToString(), emailSenderResponse.message.ToString());
                    return View();
                }
                return LocalRedirect("/Account/ForgotPasswordConfirmation");
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                ResetPasswordViewModel Input = new ResetPasswordViewModel
                {
                    Code = code
                };
                return View(Input);
            }
        }

        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel Input)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError("Error : ", "User does not exist.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return LocalRedirect("/Account/ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IsNotAllowed(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                //generate token for activtion 
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.Action(
                //    "/Account/ConfirmEmail",
                //    pageHandler: null,
                //    values: new { userId = user.Id, code },
                //    protocol: Request.Scheme);
                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { code, email = user.Email }, Request.Scheme);

                //Call Activation Template
                var confirmAccountModel = new ConfirmAccountEmailViewModel(callbackUrl);
                string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/ConfirmAccount/ConfirmAccountEmail.cshtml", confirmAccountModel);

                EmailSenderResponse emailSenderResponse = await _emailSenderService.SendEmail
                    (
                    new EmailSenderRequest()
                    {
                        senderName = "E-commerece",
                        from = "info@Example.com",
                        subject = "Resend Email Confirmation",
                        to = user.Email,
                        receiverName = user.name,
                        plainTextContent = "Please confirm your account.",
                        htmlContent = body
                    });
                ModelState.AddModelError(string.Empty, "We have sent you email to active you account.");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error white trying to send you reactivation token, please contact support team.");
            }
            return View();
        }

        [AllowAnonymous, AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailnUse(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {Email} is already in use.");
            }
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

    }
}
