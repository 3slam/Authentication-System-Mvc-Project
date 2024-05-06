using LOGWITHGOOFLE.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;

namespace LOGWITHGOOFLE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult x()
        {
            return View();
        }

        [HttpPost]
        public IActionResult x(UpdateUserInfoViewModel model)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Upload(ImageUploadViewModel imageUploadViewModel )
        {
            return Json(imageUploadViewModel.ImageFile.ToString());
        }
    }
}
