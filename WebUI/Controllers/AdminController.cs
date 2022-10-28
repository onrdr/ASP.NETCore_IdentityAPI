using DataAccess.Models;
using DataAccess.Models.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
            : base(userManager, null, roleManager)
        {
        }
        public IActionResult Index()
        {
            AppUser user = CurrentUser;
            UserViewModel model = user.Adapt<UserViewModel>();

            return View(model);
        }

        #region List Roles
        public IActionResult Roles()
        {
            return View(RoleManager.Roles.ToList());
        }
        #endregion

        #region List Users
        public IActionResult Users()
        {
            return View(UserManager.Users.ToList());
        }
        #endregion

        #region Create Role
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleViewModel model)
        {
            var role = new AppRole
            {
                Name = model.Name
            };
            IdentityResult result = await RoleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["message"] = "Created Successfully";
                return RedirectToAction("Roles");
            }
            TempData["message"] = "An Error Occured while Deleting the Role";
            AddModelError(result);
            return View();
        }
        #endregion

        #region Edit Role
        public async Task<IActionResult> RoleEdit(string id)
        {
            AppRole role = await RoleManager.FindByIdAsync(id);

            return View(role.Adapt<RoleViewModel>());
        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleViewModel model)
        {
            AppRole role = await RoleManager.FindByIdAsync(model.Id);

            if (role != null)
            {
                role.Name = model.Name;
                var result = await RoleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["message"] = "Updated Successfully";
                    return RedirectToAction(nameof(Roles));
                }
                AddModelError(result);
                TempData["message"] = "An Error Occured while Updating the Role";
            }
            return RedirectToAction(nameof(Roles));
        }
        #endregion

        #region Delete Role
        public async Task<IActionResult> RoleDelete(string id)
        {
            AppRole role = await RoleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["message"] = "Deleted Successfully";
                    return RedirectToAction(nameof(Roles));
                }
                TempData["message"] = "An Error Occured while Deleting the Role";
            }
            return RedirectToAction(nameof(Roles));
        }
        #endregion

        #region Assign Role
        public async Task<IActionResult> RoleAssign(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            ViewBag.userName = user.UserName;
            TempData["Id"] = id;

            var roles = await RoleManager.Roles.ToListAsync();
            var userRoles = await UserManager.GetRolesAsync(user);
            var roleAssignViewModels = new List<RoleAssignViewModel>();

            foreach (var role in roles)
            {
                var r = new RoleAssignViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Exist = userRoles.Contains(role.Name)
                };
                roleAssignViewModels.Add(r);
            }
            return View(roleAssignViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> model)
        {
            var user = await UserManager.FindByIdAsync(TempData["Id"].ToString());
            foreach (var item in model)
            {
                if (item.Exist)
                {
                    await UserManager.AddToRoleAsync(user, item.RoleName);
                    TempData["status"] = "Updated Successfully";
                    continue;
                }                
                await UserManager.RemoveFromRoleAsync(user, item.RoleName);
                TempData["status"] = "Updated Successfully";
            }
            return RedirectToAction("Users");
        }
        #endregion
    }
}
