using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid courseId);
        Task<Course?> GetCourseWithDetailsAsync(Guid courseId);
        Task<bool> CourseExistsAsync(Guid courseId);





        // New methods for pending courses
        Task<List<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}
