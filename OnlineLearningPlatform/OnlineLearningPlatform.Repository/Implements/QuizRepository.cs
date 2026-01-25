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

        // Methods for teacher
        public async Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId)
        {
            return await _db.Courses.Where(c => c.TeacherId == teacherId).ToListAsync();
        }

        public async Task<List<Section>> GetSectionsByCourseIdAsync(Guid courseId)
        {
            return await _db.Sections.Where(s => s.CourseId == courseId).ToListAsync();
        }

        public async Task<List<Lesson>> GetLessonsBySectionIdAsync(int sectionId)
        {
            return await _db.Lessons.Where(l => l.SectionId == sectionId).ToListAsync();
        }

        // Methods for student
        public async Task<Quiz?> GetQuizByLessonIdAsync(int lessonId)
        {
            return await _db.Quizzes.FirstOrDefaultAsync(q => q.LessonId == lessonId);
        }

        public async Task<QuizAttempt?> GetQuizAttemptByIdAsync(Guid attemptId)
        {
            return await _db.QuizAttempts
                .Include(a => a.Quiz)
                .FirstOrDefaultAsync(a => a.AttemptId == attemptId);
        }

        public async Task<List<QuizAttempt>> GetQuizAttemptsByUserIdAsync(string userId)
        {
            return await _db.QuizAttempts
                .Include(a => a.Quiz)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AttemptedAt)
                .ToListAsync();
        }

        public async Task<QuizAttempt?> GetQuizAttemptWithDetailsAsync(Guid attemptId)
        {
            return await _db.QuizAttempts
                .Include(a => a.Quiz)
                .ThenInclude(q => q.Questions)
                .FirstOrDefaultAsync(a => a.AttemptId == attemptId);
        }

        public async Task<List<QuizAnswer>> GetQuizAnswersByAttemptIdAsync(Guid attemptId)
        {
            return await _db.QuizAnswers
                .Where(ans => ans.AttemptId == attemptId)
                .ToListAsync();
        }
    }
}