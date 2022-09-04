using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Helpers;
using VetManage.Web.Models;

namespace VetManage.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Try to login
                var result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    // If the user was trying to access an area
                    // That is only allowed to signed in users
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        // Redirect to the area the user wa trying to access
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Failed to login");

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
