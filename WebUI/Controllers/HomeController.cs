using DataAccess.Models;
using DataAccess.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<AppUser> UserManager { get; }
        private SignInManager<AppUser> SignInManager { get; }
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindByEmailAsync(loginModel.EmailAddress); 
                if (user != null)
                {
                    if (await UserManager.IsLockedOutAsync(user))
                        ModelState.AddModelError("", "Your account has been locked. Please try again later");

                    await SignInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await SignInManager.PasswordSignInAsync(user, loginModel.Password, loginModel.RememberMe, true);

                    if (result.Succeeded)
                    {
                        await UserManager.ResetAccessFailedCountAsync(user);

                        if (TempData["ReturnUrl"] != null)
                            return Redirect(TempData["ReturnUrl"].ToString());

                        return RedirectToAction("Index", "Member");
                    }
                    else
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
                    }
                }
                else 
                    ModelState.AddModelError(nameof(LoginViewModel.EmailAddress), "User not found");                
            }
            return View(loginModel);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    UserName = registerModel.Username,
                    Email = registerModel.EmailAddress,
                    PhoneNumber = registerModel.PhoneNumber
                };

                var result = await UserManager.CreateAsync(user, registerModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(registerModel);
        }
    }
}