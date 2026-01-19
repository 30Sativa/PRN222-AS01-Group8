using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineLearningPlatform.Mvc.Models;
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
            return  View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var succes = await _authService.LoginAsync(
                model.Email,
                model.Password,
                model.RememberMe);
            if (!succes)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult>   Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");

        }
    }
   }
