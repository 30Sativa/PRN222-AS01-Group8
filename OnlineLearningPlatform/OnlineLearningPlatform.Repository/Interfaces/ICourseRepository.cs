using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid courseId);
        Task<Course?> GetCourseWithDetailsAsync(Guid courseId);
        Task<bool> CourseExistsAsync(Guid courseId);

        // Teacher methods
        Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId);
        Task<Course?> GetCourseWithStatisticsAsync(Guid courseId, string teacherId);
    }
}
