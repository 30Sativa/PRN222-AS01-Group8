using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    [Authorize(Roles = RolesNames.Instructor)]
    public class TeacherController : Controller
    {
        private readonly ICourseService _courseService;

        public TeacherController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: Teacher/Index - Quản lý danh sách khóa học của giảng viên
        public async Task<IActionResult> Index()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var courses = await _courseService.GetTeacherCoursesAsync(teacherId);

            return View(courses);
        }
    }
}
