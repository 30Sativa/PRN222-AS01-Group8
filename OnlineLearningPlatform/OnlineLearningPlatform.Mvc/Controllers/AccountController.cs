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

                return RedirectToAction("Index", "Home");
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
        public IActionResult LoginWithGoogle(string returnUrl)
        {
            returnUrl ??=  Url.Content("~/");
            var redictUrl = Url.Action("GoogleCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redictUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string returnUrl)
        {
            returnUrl ??= Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //Thử login nếu đã từng login Google
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            //Lấy info từ Google
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (email == null)
            {
                return RedirectToAction("Login");
            }

            //Kiểm tra user đã tồn tại theo Email chưa
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

                // GÁN ROLE MẶC ĐỊNH
                await _signInManager.UserManager.AddToRoleAsync(user, RolesEnum.Student);
            }

            //liên kết Google ↔ User
            await _signInManager.UserManager.AddLoginAsync(user, info);

            //Sign in
            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect(returnUrl);
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

        public async Task<IActionResult>   Logout()
        {
            await _authService.LogoutAsync();
            Console.WriteLine("User logged out successfully.");
            return RedirectToAction("Index", "Home");

        }
    }
   }
