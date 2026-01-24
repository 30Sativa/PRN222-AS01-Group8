using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _db;
        public QuizRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(Quiz quiz) => await _db.Quizzes.AddAsync(quiz);

        public async Task<Quiz> GetQuizWithQuestionsAsync(int id)
        {
            return await _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(ques => ques.QuizAnswers)
                .FirstOrDefaultAsync(q => q.QuizId == id);
        }
    }
}