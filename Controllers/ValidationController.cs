 

using LOGWITHGOOFLE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LOGWITHGOOFLE.Controllers
{
    public class ValidationController : Controller
    {
        public readonly UserManager<AppUser> _userManager;
        public readonly SignInManager<AppUser> _signInManager;

        public ValidationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> IsUserUnique(string UserName)
        {
            // Check If the UserName Id is Already in the Database
            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {UserName} is already in use.");
            }
        }
    }
}
