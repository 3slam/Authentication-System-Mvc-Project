using LOGWITHGOOFLE.Models;
 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
 
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using LOGWITHGOOFLE.Service;
using System.Text.Encodings.Web;
using LOGWITHGOOFLE.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;

namespace LOGWITHGOOFLE.Controllers
{
    public class AccountController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public readonly UserManager<AppUser> _userManager;
        public readonly SignInManager<AppUser> _signInManager;
        private readonly ISenderEmail _emailSender;

        public AccountController ( IWebHostEnvironment webHostEnvironment ,UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ISenderEmail emailSender)
        {
            _webHostEnvironment = webHostEnvironment; 
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Register(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl; 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(SignInViewModel model,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string imageUrl = "\\Images\\defaultUserProfile.png";
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProfileImagePath = @"Images\ProfileImages";
                    string finalPath = Path.Combine(wwwRootPath, ProfileImagePath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                 imageUrl = @"\" + ProfileImagePath + @"\" + fileName;
                }

                AppUser identityUser = new AppUser
                    {
                        FullName = model.FullName,
                        ProfileImge = imageUrl,
                        UserName = model.UserName,
                        Email = model.Email,

                    };

                    var result = await _userManager.CreateAsync(identityUser, model.Password);

                    if (result.Succeeded)
                    {

                    var claims = new List<Claim> { new Claim("ProfileImage", imageUrl) };
                    await _userManager.AddClaimsAsync(identityUser, claims);
                    await SendConfirmationEmail(model.Email, identityUser);
                        return View("RegistrationSuccessful");
                    }

                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegistrationSuccessful()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user!=null && !user.EmailConfirmed)
                {
                    await SendConfirmationEmail(user.Email, user);
                    return View("RegistrationSuccessful");
                }
                if (user != null)
                {

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        // Handle successful login
                        // Check if the ReturnUrl is not null and is a local URL
                        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                ModelState.AddModelError("Email", "Invalid Email or Password");
            }

            return View(model);
        }


        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task SendConfirmationEmail(string? email, AppUser? user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var ConfirmationLink = Url.Action("ConfirmEmail", "Account",
            new { UserId = user.Id, Token = token }, protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(ConfirmationLink)}'>clicking here</a>.", true);
        }
        private async Task SendForgotPasswordEmail(string? email, AppUser? user)
        {
           
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordResetLink = Url.Action("ResetPassword", "Account",
                    new { Email = email, Token = token }, protocol: HttpContext.Request.Scheme);

          await _emailSender.SendEmailAsync(email, "Reset Your Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(passwordResetLink)}'>clicking here</a>.", true);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
        {
            if (UserId == null || Token == null)
            {
                ViewBag.Message = "The link is Invalid or Expired";
            }

            
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {UserId} is Invalid";
                return View("NotFound");
            }

           
            var result = await _userManager.ConfirmEmailAsync(user, Token);
            if (result.Succeeded)
            {
                ViewBag.Message = "Thank you for confirming your email,now You can log In.";
                return View();
            }
          
            ViewBag.Message = "Email cannot be confirmed";
            return View();
        }



        [HttpGet]
       
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    await SendForgotPasswordEmail(user.Email, user);
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
                ModelState.AddModelError("Email", "Invalid Email or Email Not Confirmed");
                return View(model);
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string Token, string Email)
        {
            if (Token == null || Email == null)
            {
                ViewBag.ErrorTitle = "Invalid Password Reset Token";
                ViewBag.ErrorMessage = "The Link is Expired or Invalid";
                return View("Error");
            }
            else
            {
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                model.Token = Token;
                model.Email = Email;
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            { 
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

            }
  
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public async Task LogInWithGoogle()
        {

            var redirectUrl = Url.Action(action: "GoogleResponse", controller: "Account");
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = redirectUrl
                }
            );

        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result?.Principal == null || !result.Succeeded)
            {
                return Json("NUll"); // Redirect to access denied page or handle accordingly
            }

            string LoginProvider = "Google";
            string ProviderKey = result?.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            string ProviderDisplayName = result?.Principal.FindFirstValue(ClaimTypes.Name);

            var loginInfo = new UserLoginInfo(LoginProvider, ProviderKey, ProviderDisplayName);

            string userEmail = result.Principal.FindFirstValue(ClaimTypes.Email);

            if (userEmail != null)
            {

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    user = new AppUser
                    {
                        ProfileImge =  "\\Images\\defaultUserProfile.png" ,
                        UserName = result.Principal.FindFirstValue(ClaimTypes.Email),
                        Email = result.Principal.FindFirstValue(ClaimTypes.Email),
                        FullName = result.Principal.FindFirstValue(ClaimTypes.GivenName),
                       
                    };
                    await _userManager.CreateAsync(user);
                }
                var resultOfAddingLogin = await _userManager.AddLoginAsync(user, loginInfo);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }


        public IActionResult LoginWithMicrosoft()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("MicrosoftResponse")
            }, MicrosoftAccountDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> MicrosoftResponse()
        {
            var result = await HttpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);

            if (result?.Principal == null || !result.Succeeded)
            {
                return Json("NUll"); // Redirect to access denied page or handle accordingly
            }

            string LoginProvider = "Microsof";
            string ProviderKey = result?.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            string ProviderDisplayName = result?.Principal.FindFirstValue(ClaimTypes.Name);

            var loginInfo = new UserLoginInfo(LoginProvider, ProviderKey, ProviderDisplayName);

            string userEmail = result.Principal.FindFirstValue(ClaimTypes.Email);

            if (userEmail != null)
            {

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    user = new AppUser
                    {
                        ProfileImge = "\\Images\\defaultUserProfile.png",
                        UserName = result.Principal.FindFirstValue(ClaimTypes.Email),
                        Email = result.Principal.FindFirstValue(ClaimTypes.Email),
                        FullName = result.Principal.FindFirstValue(ClaimTypes.GivenName),
                      
                    };
                    await _userManager.CreateAsync(user);
                }
                var resultOfAddingLogin = await _userManager.AddLoginAsync(user, loginInfo);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ProfileUserInfo(string userName)
        {
            var user = _userManager.FindByNameAsync(userName).Result;
            return View(user);
        }

    }

}
