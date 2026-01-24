using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface ILessonService
    {
        Task<LessonDto?> GetLessonDetailAsync(int lessonId, string userId);
    }
}
