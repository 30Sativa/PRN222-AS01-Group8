using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<Quiz> GetQuizWithQuestionsAsync(int id);
        Task AddAsync(Quiz quiz);
        
        // Methods for teacher
        Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId);
        Task<List<Section>> GetSectionsByCourseIdAsync(Guid courseId);
        Task<List<Lesson>> GetLessonsBySectionIdAsync(int sectionId);
        
        // Methods for student
        Task<Quiz?> GetQuizByLessonIdAsync(int lessonId);
        Task<QuizAttempt?> GetQuizAttemptByIdAsync(Guid attemptId);
        Task<List<QuizAttempt>> GetQuizAttemptsByUserIdAsync(string userId);
        Task<QuizAttempt?> GetQuizAttemptWithDetailsAsync(Guid attemptId);
        Task<List<QuizAnswer>> GetQuizAnswersByAttemptIdAsync(Guid attemptId);
    }
}