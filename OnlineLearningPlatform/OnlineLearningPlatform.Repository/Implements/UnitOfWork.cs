using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Quizzes = new QuizRepository(_db);
            QuizAttempts = new QuizAttemptRepository(_db);
            Courses = new CourseRepository(_db);
            Lessons = new LessonRepository(_db);
        }

        public IQuizRepository Quizzes { get; private set; }
        public IQuizAttemptRepository QuizAttempts { get; private set; }
        public ICourseRepository Courses { get; private set; }
        public ILessonRepository Lessons { get; private set; }

        public async Task SaveAsync() => await _db.SaveChangesAsync();
        public void Dispose() => _db.Dispose();
    }
}