using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizAttempt> SubmitQuizAsync(string userId, int quizId, Dictionary<int, string> answers);
        Task<bool> CanUserAttemptAsync(string userId, int quizId);
    }
}