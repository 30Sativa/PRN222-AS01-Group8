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
    }
}