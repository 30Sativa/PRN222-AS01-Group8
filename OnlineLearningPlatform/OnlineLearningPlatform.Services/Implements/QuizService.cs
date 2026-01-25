using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class QuizService : IQuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public QuizService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<bool> CanUserAttemptAsync(string userId, int quizId)
        {
            var count = await _context.QuizAttempts.CountAsync(a => a.UserId == userId && a.QuizId == quizId);
            return count < 3;
        }

        public async Task<QuizAttempt> SubmitQuizAsync(string userId, int quizId, Dictionary<int, string> answers)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            int correctCount = 0;
            var attemptId = Guid.NewGuid();

            var attempt = new QuizAttempt
            {
                AttemptId = attemptId,
                QuizId = quizId,
                UserId = userId,
                AttemptedAt = DateTime.Now
            };

            foreach (var question in quiz.Questions)
            {
                string studentAns = answers.ContainsKey(question.QuestionId) ? answers[question.QuestionId] : "";

              
                _context.QuizAnswers.Add(new QuizAnswer
                {
                    AnswerId = Guid.NewGuid(),
                    AttemptId = attemptId,
                    QuestionId = question.QuestionId,
                    UserAnswer = studentAns
                });

                if (!string.IsNullOrEmpty(studentAns) &&
                    studentAns.Trim().ToLower() == question.CorrectAnswer.Trim().ToLower())
                {
                    correctCount++;
                }
            }

            double score = (double)correctCount / quiz.Questions.Count * 10;
            attempt.Score = Math.Round(score, 2);

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync(); 

            return attempt;
        }

        // Methods for teacher
        public async Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId)
        {
            return await _unitOfWork.Quizzes.GetCoursesByTeacherIdAsync(teacherId);
        }

        public async Task<List<object>> GetSectionsByCourseIdAsync(Guid courseId)
        {
            var sections = await _unitOfWork.Quizzes.GetSectionsByCourseIdAsync(courseId);
            return sections.Select(s => (object)new { s.SectionId, s.Title }).ToList();
        }

        public async Task<List<object>> GetLessonsBySectionIdAsync(int sectionId)
        {
            var lessons = await _unitOfWork.Quizzes.GetLessonsBySectionIdAsync(sectionId);
            return lessons.Select(l => (object)new { l.LessonId, l.Title }).ToList();
        }

        public async Task CreateQuizAsync(Quiz quiz)
        {
            await _unitOfWork.Quizzes.AddAsync(quiz);
            await _unitOfWork.SaveAsync();
        }

        // Methods for student
        public async Task<Quiz?> GetQuizByLessonIdAsync(int lessonId)
        {
            return await _unitOfWork.Quizzes.GetQuizByLessonIdAsync(lessonId);
        }

        public async Task<Quiz?> GetQuizWithQuestionsAsync(int quizId)
        {
            return await _unitOfWork.Quizzes.GetQuizWithQuestionsAsync(quizId);
        }

        public async Task<QuizAttempt?> GetQuizAttemptByIdAsync(Guid attemptId)
        {
            return await _unitOfWork.Quizzes.GetQuizAttemptByIdAsync(attemptId);
        }

        public async Task<List<QuizAttempt>> GetQuizAttemptHistoryByUserIdAsync(string userId)
        {
            return await _unitOfWork.Quizzes.GetQuizAttemptsByUserIdAsync(userId);
        }

        public async Task<QuizAttempt?> GetQuizAttemptWithDetailsAsync(Guid attemptId)
        {
            return await _unitOfWork.Quizzes.GetQuizAttemptWithDetailsAsync(attemptId);
        }

        public async Task<List<QuizAnswer>> GetQuizAnswersByAttemptIdAsync(Guid attemptId)
        {
            return await _unitOfWork.Quizzes.GetQuizAnswersByAttemptIdAsync(attemptId);
        }
    }
}