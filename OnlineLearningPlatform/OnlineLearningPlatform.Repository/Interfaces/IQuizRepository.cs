using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<Quiz> GetQuizWithQuestionsAsync(int id);
        Task AddAsync(Quiz quiz);
    }
}