using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Mvc.Models;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;

        public ProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await _authService.GetMyProfileAsync(userId);

            var vm = new ProfileViewModel
            {
                Email = response.Email,
                FullName = response.FullName,
                PhoneNumber = response.PhoneNumber
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileAsync(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = new ProfileViewModel
                {
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Email = User.Identity?.Name
                };

                return View("Index", vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = new UpdateUserProfileRequest
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            await _authService.UpdateProfileAsync(userId, request);

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công";

            return RedirectToAction(nameof(Index));
        }
    }
}
