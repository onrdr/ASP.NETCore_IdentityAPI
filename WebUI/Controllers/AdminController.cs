using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private UserManager<AppUser> UserManager { get; }

        public AdminController(UserManager<AppUser> userManager)
        {
            UserManager = userManager;
        }

        public IActionResult Index()
        {
            return View(UserManager.Users.ToList());
        }
    }
}
