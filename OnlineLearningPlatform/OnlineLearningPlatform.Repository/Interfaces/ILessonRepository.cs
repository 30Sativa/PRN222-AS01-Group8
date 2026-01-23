using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface ILessonRepository
    {
        Task<Lesson?> GetLessonByIdAsync(int lessonId);
        Task<bool> IsUserEnrolledInLessonCourseAsync(string userId, int lessonId);
    }
}
