using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Calories_Life_2.DAL;
using Calories_Life_2.ViewModels;
using Calories_Life_2.Models;
using System.Collections;
using System.Collections.Generic;

namespace Calories_Life_2.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private CaloriesLifeContext _context;

        public AccountController()
        {
            _context = new CaloriesLifeContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Account//Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.ReturnUrl = Url.Action("IndexUserMenu", "UserMenu");

            return View();
        }

        // POST: Account//Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("IndexUserMenu", "UserMenu");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("loginerror", "Invalid login attempt.");
                    return View(model);
            }
        }

        // GET: Account//Register
        [AllowAnonymous]
        public ActionResult Register(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ErrorEmailIsAlreadyTaken ? "Email is already taken"
                : message == ManageMessageId.Fail ? "Something goes wrong"
                : message == ManageMessageId.NotRegisterYetFail ? "You need to register your account first - try now"
                : "";

            ViewBag.ReturnUrl = Url.Action("Form", "CaloriesCounter");

            return View();
        }

        // POST: Account//Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Form", "CaloriesCounter");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl, string registerOrLogin)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl, registerOrLogin }));
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl, string registerOrLogin)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);            
                case SignInStatus.Failure:
                default:
                    if (registerOrLogin != "login")
                    {
                        // If the user does not have an account, create account with external provider login
                        // in reality, we might ask for providing email (+ confirming it)
                        // we also need some error checking logic (ie. verifiaction if user doesn't already exist)

                        var user = new ApplicationUser
                        {
                            Email = loginInfo.Email,
                            UserName = loginInfo.Email
                        };

                        var registrationResult = await UserManager.CreateAsync(user);
                        if (registrationResult.Succeeded)
                        {
                            registrationResult = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                            if (registrationResult.Succeeded)
                            {
                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                return RedirectToLocal(returnUrl);
                            }
                            else
                                return RedirectToAction("Register", "Account", new { Message = ManageMessageId.Fail });
                        }
                        else
                        {
                            List<string> errors = new List<string>();

                            foreach (var error in registrationResult.Errors)
                                errors.Add(error);

                            if (errors[1] != null && errors[1] == "Email '" + user.Email + "' is already taken.")
                                return RedirectToAction("Register", "Account", new { Message = ManageMessageId.ErrorEmailIsAlreadyTaken });
                            else
                                return RedirectToAction("Register", "Account", new { Message = ManageMessageId.Fail });
                        }
                    }
                    else
                        return RedirectToAction("Register", "Account", new { Message = ManageMessageId.NotRegisterYetFail });
            }
        }

        public enum ManageMessageId
        {
            ErrorEmailIsAlreadyTaken,
            Fail,
            NotRegisterYetFail
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }

        }
        #endregion
    }
}