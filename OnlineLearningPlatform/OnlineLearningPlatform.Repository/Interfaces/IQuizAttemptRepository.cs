using OnlineLearningPlatform.Models.Entities;
using System.Linq.Expressions;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IQuizAttemptRepository
    {
        Task AddAsync(QuizAttempt attempt);
        Task<IEnumerable<QuizAttempt>> GetAllAsync(Expression<Func<QuizAttempt, bool>> filter);
        Task<int> CountAsync(Expression<Func<QuizAttempt, bool>> filter);
    }
}