using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Mvc.Models;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(
            IProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET: Profile/Index - Hiển thị trang profile
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var profile = await _profileService.GetProfileAsync(userId);
            if (profile == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                FullName = profile.FullName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                CourseCount = profile.CourseCount,
                ArticleCount = profile.ArticleCount,
                UserId = profile.UserId
            };

            return View(viewModel);
        }

        // POST: Profile/Update - Cập nhật thông tin profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var profile = await _profileService.GetProfileAsync(userId);
                    if (profile != null)
                    {
                        model.Email = profile.Email;
                        model.PhoneNumber = profile.PhoneNumber;
                        model.CourseCount = profile.CourseCount;
                        model.ArticleCount = profile.ArticleCount;
                        model.UserId = profile.UserId;
                    }
                }
                return View("Index", model);
            }

            var userId2 = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId2))
            {
                return Unauthorized();
            }

            // Cập nhật thông tin (FullName và PhoneNumber)
            var fullNameResult = await _profileService.UpdateFullNameAsync(userId2, model.FullName);
            var phoneResult = await _profileService.UpdatePhoneAsync(userId2, model.PhoneNumber ?? "");

            if (fullNameResult.Succeeded && phoneResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                var allErrors = fullNameResult.Errors.Concat(phoneResult.Errors);
                foreach (var error in allErrors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Reload statistics và đảm bảo Email, PhoneNumber được set lại
            var profileAfter = await _profileService.GetProfileAsync(userId2);
            if (profileAfter != null)
            {
                model.Email = profileAfter.Email;
                model.PhoneNumber = profileAfter.PhoneNumber;
                model.CourseCount = profileAfter.CourseCount;
                model.ArticleCount = profileAfter.ArticleCount;
                model.UserId = profileAfter.UserId;
            }

            return View("Index", model);
        }
    }
}
