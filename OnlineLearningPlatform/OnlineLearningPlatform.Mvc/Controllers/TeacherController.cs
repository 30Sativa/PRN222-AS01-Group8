using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Mvc.Models.Teacher;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    [Authorize(Roles = RolesNames.Instructor)]
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        // GET: Teacher/Index - Quản lý danh sách khóa học của giảng viên
        public async Task<IActionResult> Index()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var courses = await _teacherService.GetTeacherCoursesAsync(teacherId);

            return View(courses);
        }

        // GET: Teacher/Create - Hiển thị form tạo khóa học
        public async Task<IActionResult> Create()
        {
            var categories = await _teacherService.GetCategoriesAsync();
            var viewModel = new CreateCourseViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Teacher/Create - Xử lý tạo khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _teacherService.GetCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
                return View(model);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var request = new CreateCourseRequest
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId
            };

            var courseId = await _teacherService.CreateCourseAsync(request, teacherId);

            TempData["SuccessMessage"] = "Tạo khóa học thành công!";
            return RedirectToAction(nameof(Edit), new { id = courseId });
        }

        // GET: Teacher/Edit/{id} - Hiển thị form chỉnh sửa khóa học
        public async Task<IActionResult> Edit(Guid id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var course = await _teacherService.GetTeacherCourseByIdAsync(id, teacherId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khóa học hoặc bạn không có quyền chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _teacherService.GetCategoriesAsync();
            var viewModel = new EditCourseViewModel
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList()
            };

            // Set CategoryId nếu có
            if (!string.IsNullOrEmpty(course.CategoryName))
            {
                var selectedCategory = categories.FirstOrDefault(c => c.CategoryName == course.CategoryName);
                if (selectedCategory != null)
                {
                    viewModel.CategoryId = selectedCategory.CategoryId;
                }
            }

            return View(viewModel);
        }

        // POST: Teacher/Edit/{id} - Xử lý cập nhật khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditCourseViewModel model)
        {
            if (id != model.CourseId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var categories = await _teacherService.GetCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
                return View(model);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var request = new UpdateCourseRequest
            {
                CourseId = model.CourseId,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId
            };

            var result = await _teacherService.UpdateCourseAsync(id, request, teacherId);

            if (result)
            {
                TempData["SuccessMessage"] = "Cập nhật khóa học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật khóa học.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Teacher/Delete/{id} - Xóa khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var result = await _teacherService.DeleteCourseAsync(id, teacherId);

            if (result)
            {
                TempData["SuccessMessage"] = "Xóa khóa học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa khóa học.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
