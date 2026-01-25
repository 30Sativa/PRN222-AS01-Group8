using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;

        public HomeController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách khóa học nổi bật (không cần userId vì chưa đăng nhập)
            var courses = await _courseService.GetAllCoursesAsync();
            
            // Lấy 6 khóa học đầu tiên để hiển thị
            var featuredCourses = courses.Take(6).ToList();

            ViewBag.FeaturedCourses = featuredCourses;
            ViewBag.TotalCourses = courses.Count;

            return View();
        }
    }
}
