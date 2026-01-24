using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Enums;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Mvc.Models;
using OnlineLearningPlatform.Services.DTO.Request.Course;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Mvc.Areas.Admin.Controllers
{
    [Area(RolesNames.Admin)]
    [Authorize(Roles = RolesNames.Admin)]
    public class CourseController : Controller
    {
        private readonly ICourseService _service;

        public CourseController(ICourseService service)
        {
            _service = service;
        }

        // GET: /Admin/Course
        public async Task<IActionResult> Index(
            CourseStatus? status,
            string? keyword)
        {
            var dtos = await _service.GetCoursesAsync(status, keyword);

            var vm = dtos.Select(x => new CourseManagerViewModel
            {
                CourseId = x.CourseId,
                Title = x.Title,
                TeacherName = x.TeacherName,
                Status = x.Status,

                PriceText = $"{x.Price:N0} VNĐ",

                StatusText = x.Status switch
                {
                    CourseStatus.Pending => "Chờ duyệt",
                    CourseStatus.Published => "Đã duyệt",
                    CourseStatus.Rejected => "Bị từ chối",
                    _ => ""
                }
            }).ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _service.ApproveAsync(
                new ApproveCourseRequest { CourseId = id });

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            await _service.RejectAsync(
                new RejectCourseRequest
                {
                    CourseId = id,
                    Reason = reason
                });

            return RedirectToAction(nameof(Index));
        }
    }
}
