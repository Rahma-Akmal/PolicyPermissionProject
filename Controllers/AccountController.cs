using App.Application.ViewModel;
using App.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PermissionApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> UserManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _UserManager = UserManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
       
        public async Task<IActionResult> Index()
        {
            var users = _UserManager.Users.ToList();

            var userRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _UserManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();

                string? roleId = null;
                if (!string.IsNullOrEmpty(roleName))
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    roleId = role?.Id;
                }

                userRoles.Add(new
                {
                    Email = user.Email,
                    Name = user.Name,
                    UserImage = user.UserImage,
                    RoleName = roleName,
                    RoleId = roleId
                });
            }


            ViewBag.Users = userRoles;
            return View();
        }
        [HttpGet]
    
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
      
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    UserImage= "heart.gif"
                };
                var result = await _UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _UserManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
       
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
       
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Account");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }
        [HttpPost]
       
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}
