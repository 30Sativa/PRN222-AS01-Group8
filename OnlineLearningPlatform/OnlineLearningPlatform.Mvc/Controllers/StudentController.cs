using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Mvc.Models;
using OnlineLearningPlatform.Services.DTO.Response;
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
        private readonly IPaymentService _paymentService;

        public StudentController(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            ILessonService lessonService,
            IPaymentService paymentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _lessonService = lessonService;
            _paymentService = paymentService;
        }

        // GET: Student/Index - Xem danh sách khóa học
        public async Task<IActionResult> Index(string? searchString)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = await _courseService.GetAllCoursesAsync(userId, searchString);

            ViewData["CurrentFilter"] = searchString;

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

            // Get course to check if it's free or paid
            var course = await _courseService.GetCourseDetailAsync(courseId, userId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Khóa học không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            // If course is free, enroll directly
            if (course.Price == 0)
            {
                var result = await _enrollmentService.EnrollInCourseAsync(userId, courseId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Đăng ký khóa học thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể đăng ký khóa học. Có thể bạn đã đăng ký rồi.";
                }
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            // For paid courses, redirect to checkout
            return RedirectToAction(nameof(Checkout), new { courseId = courseId });
        }

        // GET: Student/Checkout/{courseId} - Trang thanh toán
        public async Task<IActionResult> Checkout(Guid courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var course = await _courseService.GetCourseDetailAsync(courseId, userId);
            if (course == null)
            {
                return NotFound();
            }

            if (course.IsEnrolled)
            {
                TempData["InfoMessage"] = "Bạn đã đăng ký khóa học này rồi.";
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            if (course.Price == 0)
            {
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            return View(course);
        }

        // POST: Student/ProcessPayment - Xử lý thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(Guid courseId, string paymentMethod = "demo")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Create payment and enrollment request
            var payment = await _enrollmentService.EnrollWithPaymentAsync(userId, courseId, paymentMethod);
            
            if (payment == null)
            {
                TempData["ErrorMessage"] = "Không thể tạo thanh toán. Có thể bạn đã đăng ký khóa học này rồi.";
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            // Process payment (demo - always success)
            var paymentProcessed = await _paymentService.ProcessPaymentAsync(payment.PaymentId, paymentMethod);
            
            if (paymentProcessed)
            {
                // Complete enrollment after successful payment
                var enrollmentCompleted = await _enrollmentService.CompleteEnrollmentAfterPaymentAsync(payment.PaymentId);
                
                if (enrollmentCompleted)
                {
                    return RedirectToAction(nameof(PaymentSuccess), new { paymentId = payment.PaymentId });
                }
            }

            return RedirectToAction(nameof(PaymentFailed), new { paymentId = payment.PaymentId });
        }

        // GET: Student/PaymentSuccess/{paymentId} - Trang thanh toán thành công
        public async Task<IActionResult> PaymentSuccess(Guid paymentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null || payment.UserId != userId)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Student/PaymentFailed/{paymentId} - Trang thanh toán thất bại
        public async Task<IActionResult> PaymentFailed(Guid paymentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null || payment.UserId != userId)
            {
                return NotFound();
            }

            return View(payment);
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
