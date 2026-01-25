using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Mvc.Models;
using OnlineLearningPlatform.Services.DTO.Request.Review;
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
        private readonly IReviewService _reviewService;

        public StudentController(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            ILessonService lessonService,
            IPaymentService paymentService,
            IReviewService reviewService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _lessonService = lessonService;
            _paymentService = paymentService;
            _reviewService = reviewService;
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

            // Load review summary và một số reviews mới nhất
            var reviewSummary = await _reviewService.GetCourseReviewSummaryAsync(id, userId);
            var recentReviews = await _reviewService.GetCourseReviewsAsync(id, userId);
            var topReviews = recentReviews.Take(5).ToList(); // Hiển thị 5 reviews đầu tiên

            var viewModel = new CourseDetailViewModel
            {
                Course = course,
                CanEnroll = !course.IsEnrolled
            };

            ViewBag.ReviewSummary = reviewSummary;
            ViewBag.TopReviews = topReviews;
            ViewBag.CanReview = !string.IsNullOrEmpty(userId) && await _reviewService.CanUserReviewCourseAsync(userId, id);

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

        // GET: Student/ReviewCourse/{courseId} - Form để tạo/sửa review
        public async Task<IActionResult> ReviewCourse(Guid courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Kiểm tra user đã đăng ký khóa học chưa
            var canReview = await _reviewService.CanUserReviewCourseAsync(userId, courseId);
            if (!canReview)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng ký khóa học trước khi đánh giá.";
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            // Lấy course info
            var course = await _courseService.GetCourseDetailAsync(courseId, userId);
            if (course == null)
            {
                return NotFound();
            }

            // Kiểm tra user đã review chưa
            var existingReview = await _reviewService.GetUserReviewForCourseAsync(userId, courseId);
            
            // Tạo model cho form
            var model = new OnlineLearningPlatform.Services.DTO.Request.Review.CreateReviewRequest
            {
                CourseId = courseId
            };

            // Nếu đã có review, populate data vào model
            if (existingReview != null)
            {
                model.Rating = existingReview.Rating ?? 0;
                model.Comment = existingReview.Comment;
            }
            
            ViewBag.Course = course;
            ViewBag.ExistingReview = existingReview;

            return View(model);
        }

        // POST: Student/CreateReview - Tạo review mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(CreateReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin.";
                return RedirectToAction(nameof(ReviewCourse), new { courseId = request.CourseId });
            }

            var review = await _reviewService.CreateReviewAsync(userId, request);
            if (review == null)
            {
                TempData["ErrorMessage"] = "Không thể tạo đánh giá. Có thể bạn đã đánh giá khóa học này rồi hoặc chưa đăng ký khóa học.";
                return RedirectToAction(nameof(CourseDetails), new { id = request.CourseId });
            }

            TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi thành công!";
            return RedirectToAction(nameof(CourseDetails), new { id = request.CourseId });
        }

        // POST: Student/UpdateReview - Cập nhật review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReview(UpdateReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin.";
                var review = await _reviewService.GetUserReviewForCourseAsync(userId, Guid.Empty);
                if (review != null)
                {
                    return RedirectToAction(nameof(ReviewCourse), new { courseId = review.CourseId });
                }
                return RedirectToAction(nameof(Index));
            }

            var updatedReview = await _reviewService.UpdateReviewAsync(userId, request);
            if (updatedReview == null)
            {
                TempData["ErrorMessage"] = "Không thể cập nhật đánh giá. Có thể bạn không có quyền chỉnh sửa đánh giá này.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Đánh giá đã được cập nhật thành công!";
            return RedirectToAction(nameof(CourseDetails), new { id = updatedReview.CourseId });
        }

        // POST: Student/DeleteReview/{reviewId} - Xóa review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId, Guid courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var deleted = await _reviewService.DeleteReviewAsync(userId, reviewId);
            if (!deleted)
            {
                TempData["ErrorMessage"] = "Không thể xóa đánh giá. Có thể bạn không có quyền xóa đánh giá này.";
            }
            else
            {
                TempData["SuccessMessage"] = "Đánh giá đã được xóa thành công!";
            }

            return RedirectToAction(nameof(CourseDetails), new { id = courseId });
        }

        // GET: Student/CourseReviews/{courseId} - Xem tất cả reviews của khóa học
        public async Task<IActionResult> CourseReviews(Guid courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviews = await _reviewService.GetCourseReviewsAsync(courseId, userId);
            var summary = await _reviewService.GetCourseReviewSummaryAsync(courseId, userId);

            var course = await _courseService.GetCourseDetailAsync(courseId, userId);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            ViewBag.ReviewSummary = summary;

            return View(reviews);
        }
    }
}
