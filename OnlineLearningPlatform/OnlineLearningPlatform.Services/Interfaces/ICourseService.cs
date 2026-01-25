using OnlineLearningPlatform.Models.Enums;
using OnlineLearningPlatform.Services.DTO.Request.Course;
using OnlineLearningPlatform.Services.DTO.Response.Course;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllCoursesAsync(string? userId = null, string? keyword = null);
        Task<List<CourseDto>> GetUserEnrolledCoursesAsync(string userId);
        Task<CourseDetailDto?> GetCourseDetailAsync(Guid courseId, string? userId = null);

        //New methods to be added
        Task<List<CourseManagerDto>> GetCoursesAsync(CourseStatus? status,string? keyword);

        Task ApproveAsync(ApproveCourseRequest request);

        Task RejectAsync(RejectCourseRequest request);
    }
}
