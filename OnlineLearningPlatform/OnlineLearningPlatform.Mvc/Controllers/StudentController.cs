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
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILessonService _lessonService;

        public StudentController(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            ILessonService lessonService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _lessonService = lessonService;
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

        // GET: Student/MyCourses - Xem khóa học đã đăng ký
        public async Task<IActionResult> MyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = await _courseService.GetUserEnrolledCoursesAsync(userId);

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

        // POST: Student/Enroll/{courseId} - Đăng ký khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(Guid courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _enrollmentService.EnrollInCourseAsync(userId, courseId);

            if (result)
            {
                TempData["SuccessMessage"] = "Đăng ký khóa học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể đăng ký khóa học. Có thể bạn đã đăng ký rồi hoặc khóa học không tồn tại.";
            }

            return RedirectToAction(nameof(CourseDetails), new { id = courseId });
        }

        // GET: Student/WatchLesson/{lessonId} - Xem video bài học
        public async Task<IActionResult> WatchLesson(int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lesson = await _lessonService.GetLessonDetailAsync(lessonId, userId);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Student/MarkAsComplete - Đánh dấu hoàn thành bài học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsComplete(int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _lessonService.MarkLessonAsCompleteAsync(userId, lessonId);

            return RedirectToAction(nameof(WatchLesson), new { lessonId = lessonId });
        }
    }
}
