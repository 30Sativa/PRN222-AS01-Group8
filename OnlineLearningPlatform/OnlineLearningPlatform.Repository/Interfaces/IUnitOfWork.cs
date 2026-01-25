using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IUnitOfWork 
    {
        IQuizRepository Quizzes { get; }
        IQuizAttemptRepository QuizAttempts { get; }
        ICourseRepository Courses { get; }
        ILessonRepository Lessons { get; }
        Task SaveAsync();
    }
}