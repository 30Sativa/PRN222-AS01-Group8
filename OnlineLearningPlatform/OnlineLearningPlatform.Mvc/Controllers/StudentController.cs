using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Mvc.Models;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    [Authorize(Roles = RolesNames.Student)]
    public class StudentController : Controller
    {
        private readonly ICourseService _courseService;

        public StudentController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: Student/Index - Xem danh sách khóa học
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = await _courseService.GetAllCoursesAsync(userId);

            var viewModel = new CourseListViewModel
            {
                Courses = courses
            };

            return View(viewModel);
        }

        // GET: Student/CourseDetails/{id} - Xem chi tiết khóa học
        public async Task<IActionResult> CourseDetails(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _courseService.GetCourseDetailAsync(id, userId);

            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new CourseDetailViewModel
            {
                Course = course,
                CanEnroll = !course.IsEnrolled
            };

            return View(viewModel);
        }
    }
}
