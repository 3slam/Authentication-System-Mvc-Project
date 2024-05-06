using LOGWITHGOOFLE.Models;
using LOGWITHGOOFLE.ViewModels;
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LOGWITHGOOFLE.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        public readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdministrationController(IWebHostEnvironment  webHostEnvironment ,  RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                bool roleExists = await _roleManager.RoleExistsAsync(roleModel?.RoleName);
                if (roleExists)
                {
                    ModelState.AddModelError("", "Role Already Exists");
                }
                else
                {
                    IdentityRole identityRole = new IdentityRole
                    {
                        Name = roleModel?.RoleName
                    };

                    IdentityResult result = await _roleManager.CreateAsync(identityRole);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(roleModel);
        }


        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            List<IdentityRole> roles = await _roleManager.Roles
                                       .Where(role => role.Name != "Admin")
                                       .ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(viewModel.Id);
                if (role == null)
                {
                    return View(viewModel);
                }
                else
                {
                    role.Id = viewModel.Id;
                    role.Name = viewModel.RoleName;
                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                }
            }


            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ModelState.AddModelError("", $"Role with Id = {roleId} cannot be found");
                return RedirectToAction("ListRoles");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("ListRoles", await _roleManager.Roles.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var users = _userManager.Users.Where(u => u.Id != currentUser.Id).ToList();
            return View(users);
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string userUserName)
        {
            var user = await _userManager.FindByNameAsync(userUserName);
            var editedUser = new EditUserViewModel
            {
                UserID = user.Id,
                FullName = user.FullName,
                Email = user.Email,
              
                UserName = user.UserName
            };
            return View(editedUser);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel editedUser)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editedUser.UserID);
                if (user == null)
                {
                    ModelState.AddModelError("", "User Not Found");
                }
                else
                {
                    user.Id = editedUser.UserID;
                    user.Email = editedUser.Email;
                    user.UserName = editedUser.UserName;
                    user.FullName = editedUser.FullName;
                    

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View(editedUser);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteUser(string UserName)
        {

            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
            {
                ModelState.AddModelError("", $"User  with Id = {UserName} cannot be found");
                return RedirectToAction("ListRoles");
            }
            else
            {

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return View("ListUsers");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model,IFormFile file)
        {
            if(ModelState.IsValid) 
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
                        ProfileImge = imageUrl,
                        UserName = model.UserName,
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(identityUser, model.Password);

                    if (result.Succeeded)
                    {                       
                        return RedirectToAction("ListUsers");
                    }

                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }

                }
                return View(model);
            }
            return View(model);
        }
    }
}
