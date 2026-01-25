using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizAttempt> SubmitQuizAsync(string userId, int quizId, Dictionary<int, string> answers);
        Task<bool> CanUserAttemptAsync(string userId, int quizId);
        
        // Methods for teacher
        Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId);
        Task<List<object>> GetSectionsByCourseIdAsync(Guid courseId);
        Task<List<object>> GetLessonsBySectionIdAsync(int sectionId);
        Task CreateQuizAsync(Quiz quiz);
        
        // Methods for student
        Task<Quiz?> GetQuizByLessonIdAsync(int lessonId);
        Task<Quiz?> GetQuizWithQuestionsAsync(int quizId);
        Task<QuizAttempt?> GetQuizAttemptByIdAsync(Guid attemptId);
        Task<List<QuizAttempt>> GetQuizAttemptHistoryByUserIdAsync(string userId);
        Task<QuizAttempt?> GetQuizAttemptWithDetailsAsync(Guid attemptId);
        Task<List<QuizAnswer>> GetQuizAnswersByAttemptIdAsync(Guid attemptId);
    }
}