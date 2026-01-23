using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllCoursesAsync(string? userId = null);
        Task<List<CourseDto>> GetUserEnrolledCoursesAsync(string userId);
        Task<CourseDetailDto?> GetCourseDetailAsync(Guid courseId, string? userId = null);
    }
}
