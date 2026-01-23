using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllCoursesAsync(string? userId = null);
    }
}
