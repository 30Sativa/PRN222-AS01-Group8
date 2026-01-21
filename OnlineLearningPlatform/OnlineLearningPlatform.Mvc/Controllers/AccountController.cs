using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineLearningPlatform.Mvc.Models;
using OnlineLearningPlatform.Services.DTO;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine("Login attempt for user: " + model.Email);
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
                Console.WriteLine("User logged in successfully.");
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

            var dto = new RegisterDto
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
            };

            var result = await _authService.RegisterAsync(dto);

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
