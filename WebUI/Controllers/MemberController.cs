using DataAccess.Models;
using DataAccess.Models.Enums;
using DataAccess.Models.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            : base(userManager, signInManager)
        {
        }

        #region Index
        public IActionResult Index()
        {
            AppUser user = CurrentUser;
            UserViewModel model = user.Adapt<UserViewModel>();

            return View(model);
        }
        #endregion

        #region Edit
        public IActionResult UserEdit()
        {
            var user = CurrentUser;
            UserViewModel userVM = user.Adapt<UserViewModel>();
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel model, IFormFile? userPicture)
        {
            ModelState.Remove("Password");
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            if (ModelState.IsValid)
            {
                var user = CurrentUser;

                if (userPicture != null && userPicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await userPicture.CopyToAsync(stream);
                    user.Picture = "/UserPicture/" + fileName;
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.City = model.City;
                user.BirthDay = model.BirthDay;
                user.Gender = (int)model.Gender;

                IdentityResult result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await UserManager.UpdateSecurityStampAsync(user);
                    await SignInManager.SignOutAsync();
                    await SignInManager.SignInAsync(user, true);
                    ViewBag.success = "true";
                }
                else
                {
                    AddModelError(result);
                }
            }

            return View(model);
        }
        #endregion

        #region Change Password      
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            AppUser user = CurrentUser;

            if (user == null)
                return View(model);

            bool exist = await UserManager.CheckPasswordAsync(user, model.PasswordOld);

            if (!exist)
            {
                ModelState.AddModelError("", "Old Password is wrong");
                return View(model);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(user, model.PasswordOld, model.PasswordNew);

            if (!result.Succeeded)
            {
                AddModelError(result);
                return View(model);
            }

            await UserManager.UpdateSecurityStampAsync(user);
            await SignInManager.SignOutAsync();
            await SignInManager.PasswordSignInAsync(user, model.PasswordNew, true, false);

            ViewBag.success = true;
            return View(model);

        }
        #endregion

        #region Logout
        public async void Logout()
        {
            await SignInManager.SignOutAsync();
        }
        #endregion

        #region Claims
        [Authorize(Policy = "AmsterdamPolicy")]
        public IActionResult AmsterdamPage()
        {
            return View();
        }

        [Authorize(Policy = "ViolencePolicy")]
        public IActionResult ViolencePage()
        {
            return View();
        }

        public async Task<IActionResult> ExchangeRedirect()
        {
            //DeleteClaims("ExpireDateExchange");

            bool result = User.HasClaim(x => x.Type == "ExpireDateExchange"); 

            var date = DateTime.Now.AddDays(30).Date.ToShortDateString();
            if (!result)
            {
                Claim expireDateExchange = new("ExpireDateExchange", date, ClaimValueTypes.String, "Internal");
                await UserManager.AddClaimAsync(CurrentUser, expireDateExchange);
                await SignInManager.SignOutAsync();
                await SignInManager.SignInAsync(CurrentUser, true);
            }
            return RedirectToAction(nameof(ExchangePage));
        }

        [Authorize(Policy = "ExchangePolicy")]
        public IActionResult ExchangePage()
        {
            return View();
        }
        #endregion

        public IActionResult AccessDenied(string ReturnUrl)
        {
            if (ReturnUrl.Contains("Violence")) 
                ViewBag.message = "Violence Content. You have to be over 18 to open this page !!!";

            else if (ReturnUrl.Contains("Amsterdam")) 
                ViewBag.message = "Amsterdam Members only !!!"; 

            else if (ReturnUrl.Contains("Exchange"))
                ViewBag.message = "30 days of trial has ended !!!";
            else
                ViewBag.message = "You dont have access to this page !!!";

            return View();
        }      
        
        private async void DeleteClaims(string claimName)
        {
            var claims = await UserManager.GetClaimsAsync(CurrentUser);
            var lastAccessedClaim = claims.FirstOrDefault(t => t.Type == claimName);
            await UserManager.RemoveClaimAsync(CurrentUser, lastAccessedClaim);
        }
    }
}

