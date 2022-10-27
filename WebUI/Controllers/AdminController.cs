using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    { 

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) 
            : base(userManager, signInManager)
        { 
        }

        public IActionResult Index()
        {
            return View(UserManager.Users.ToList());
        }
    }
}
