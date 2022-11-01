using Business.Utilities.Helper;
using DataAccess.Models;
using DataAccess.Models.ViewModels;
using Mapster; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; 

namespace WebUI.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            : base(userManager, signInManager)
        {
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return View();

            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");
            else
                return RedirectToAction("Index", "Member");
        }

        #region Login
        public IActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            AppUser user = await UserManager.FindByEmailAsync(loginModel.EmailAddress);

            if (user == null)
            {
                ModelState.AddModelError(nameof(LoginViewModel.EmailAddress), "User not found");
                return View(loginModel);
            }

            if (await UserManager.IsLockedOutAsync(user))
                ModelState.AddModelError("", "Your account has been locked. Please try again later");

            await SignInManager.SignOutAsync();
            Microsoft.AspNetCore.Identity.SignInResult result = await SignInManager.PasswordSignInAsync(user, loginModel.Password, loginModel.RememberMe, true);

            if (!result.Succeeded)
            {
                await UserManager.AccessFailedAsync(user);
                int fail = await UserManager.GetAccessFailedCountAsync(user);
                ModelState.AddModelError("", $"Login Failed #{fail}");

                if (fail == 3)
                {
                    await UserManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(20)));
                    ModelState.AddModelError("", "Login Failed #3: Your account has been locked for 20 minutes");
                }
                else
                    ModelState.AddModelError("", "Invalid Username or password");
                return View(loginModel);
            }

            await UserManager.ResetAccessFailedCountAsync(user);
            if (TempData["ReturnUrl"] != null)
                return Redirect(TempData["ReturnUrl"].ToString());

            if (user.UserName == "admin")
            {
                return RedirectToAction("Index","Admin");
            }

            return RedirectToAction("Index", "Member");                 
        }
        #endregion

        #region SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = userViewModel.Adapt<AppUser>();

                var result = await UserManager.CreateAsync(user, userViewModel.Password);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Login));

                AddModelError(result);
            }
            return View(userViewModel);
        }
        #endregion

        #region ResetPassword
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordResetViewModel passwordResetModel)
        {
            AppUser user = await UserManager.FindByEmailAsync(passwordResetModel.EmailAddress);

            if (user == null)
            {
                ModelState.AddModelError("", "This email address is not registered");
                return View(passwordResetModel);
            }
            string passwordResetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
            string? passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
            {
                userId = user.Id,
                token = passwordResetToken,
            }, HttpContext.Request.Scheme);

            PasswordReset.PasswordResetSendEmail(passwordResetLink, user.Email);
            ViewBag.status = "success";
            return View(passwordResetModel);
        }

        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("PasswordNew")] PasswordResetViewModel passwordResetModel)
        {
            string userId = TempData["userId"].ToString();
            string token = TempData["token"].ToString();

            AppUser user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("", "An error occured, please try again later");
                return View(passwordResetModel);
            }

            IdentityResult result = await UserManager.ResetPasswordAsync(user, token, passwordResetModel.PasswordNew);

            if (!result.Succeeded)
            {
                AddModelError(result);
                return View(passwordResetModel);
            }

            await UserManager.UpdateSecurityStampAsync(user);
            ViewBag.status = "success";
            return View(passwordResetModel);
        }
        #endregion

        #region ContactInfo
        public IActionResult Contact()
        {
            return View();
        }
        #endregion
    }
}