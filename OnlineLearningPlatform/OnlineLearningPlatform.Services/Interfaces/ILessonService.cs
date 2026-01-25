using OnlineLearningPlatform.Services.DTO.Response.Course;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface ILessonService
    {
        Task<LessonDto?> GetLessonDetailAsync(int lessonId, string userId);
        Task MarkLessonAsCompleteAsync(string userId, int lessonId);
        Task<bool> IsLessonCompletedAsync(string userId, int lessonId);
    }
}
