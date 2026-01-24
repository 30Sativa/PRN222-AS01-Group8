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
    }
}
