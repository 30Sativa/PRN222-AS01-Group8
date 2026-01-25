using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface ILessonRepository
    {
        Task<Lesson?> GetLessonByIdAsync(int lessonId);
        Task<bool> IsUserEnrolledInLessonCourseAsync(string userId, int lessonId);
        Task<LessonProgress?> GetLessonProgressAsync(string userId, int lessonId);
        Task MarkLessonCompleteAsync(string userId, int lessonId);
        Task<List<int>> GetCompletedLessonIdsAsync(string userId, Guid courseId);
    }
}
