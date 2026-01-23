using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Mvc.Models;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(IAuthService authService, SignInManager<ApplicationUser> signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
        }


        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
  
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _authService.LoginAsync(
                model.Email,
                model.Password,
                model.RememberMe);


            if (result.Succeeded)
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
                return await RedirectByRoleAsync(user);
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", "Email chưa được xác nhận");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Tài khoản bị khóa");
            }
            else
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleCallback", "Account");
            var properties = _signInManager
                .ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return Challenge(properties, "Google");
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallback()
        {


            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (email == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _signInManager.UserManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = name,
                    EmailConfirmed = true
                };

                var createResult = await _signInManager.UserManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }

            // ensure role
            if (!await _signInManager.UserManager.IsInRoleAsync(user, RolesNames.Student))
            {
                await _signInManager.UserManager.AddToRoleAsync(user, RolesNames.Student);
            }

            // ensure external login linked
            var logins = await _signInManager.UserManager.GetLoginsAsync(user);
            if (!logins.Any(l => l.LoginProvider == info.LoginProvider))
            {
                await _signInManager.UserManager.AddLoginAsync(user, info);
            }

            // SIGN IN SAU KHI ROLE OK
            await _signInManager.SignInAsync(user, isPersistent: false);

            
            return await RedirectByRoleAsync(user);
        }

        private async Task<IActionResult> RedirectByRoleAsync(ApplicationUser user)
        {
            if (await _signInManager.UserManager.IsInRoleAsync(user, RolesNames.Admin))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (await _signInManager.UserManager.IsInRoleAsync(user, RolesNames.Instructor))
            {
                return RedirectToAction("Index", "Instructor");
            }

            // Student redirect to Student/Index (có navbar)
            return RedirectToAction("Index", "Student");
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            var request = new RegisterRequest
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };



            var result = await _authService.RegisterAsync(request);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login", "Account");

        }
    }
}
