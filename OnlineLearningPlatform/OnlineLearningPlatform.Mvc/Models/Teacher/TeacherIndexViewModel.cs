using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Mvc.Models.Teacher
{
    /// <summary>
    /// ViewModel cho trang Index của giáo viên: khóa chờ duyệt + khóa đã xuất bản.
    /// </summary>
    public class TeacherIndexViewModel
    {
        public List<TeacherCourseDto> PendingCourses { get; set; } = new();
        public List<TeacherCourseDto> PublishedCourses { get; set; } = new();
    }
}
