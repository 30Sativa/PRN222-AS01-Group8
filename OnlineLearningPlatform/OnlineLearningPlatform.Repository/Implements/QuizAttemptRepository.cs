using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class QuizAttemptRepository : IQuizAttemptRepository
    {
        private readonly ApplicationDbContext _db;
        public QuizAttemptRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(QuizAttempt attempt) => await _db.QuizAttempts.AddAsync(attempt);

        public async Task<int> CountAsync(Expression<Func<QuizAttempt, bool>> filter) => await _db.QuizAttempts.CountAsync(filter);

        public async Task<IEnumerable<QuizAttempt>> GetAllAsync(Expression<Func<QuizAttempt, bool>> filter)
        {
            return await _db.QuizAttempts.Where(filter).ToListAsync();
        }
    }
}