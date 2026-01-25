using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface ILessonProgressRepository
    {
        /// <summary>
        /// Lấy tất cả tiến độ của học viên trong 1 khóa học
        /// </summary>
        Task<List<LessonProgress>> GetStudentProgressInCourseAsync(string studentId, Guid courseId);
        
        /// <summary>
        /// Lấy tiến độ của học viên trong 1 bài học cụ thể
        /// </summary>
        Task<LessonProgress?> GetLessonProgressAsync(string studentId, int lessonId);
    }
}
