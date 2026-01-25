using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Models.Enums;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    /// <summary>
    /// Repository implementation cho Teacher - Tách riêng để tránh conflict
    /// </summary>
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId)
        {
            return await GetCoursesByTeacherIdAndStatusAsync(teacherId, null);
        }

        public async Task<List<Course>> GetCoursesByTeacherIdAndStatusAsync(string teacherId, CourseStatus? status = null)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Enrollments)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .Where(c => c.TeacherId == teacherId);

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseWithStatisticsAsync(Guid courseId, string teacherId)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.User)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .Where(c => c.CourseId == courseId && c.TeacherId == teacherId)
                .FirstOrDefaultAsync();
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            try
            {
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCourseAsync(Guid courseId, string teacherId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null)
                return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsTeacherOwnsCourseAsync(Guid courseId, string teacherId)
        {
            return await _context.Courses
                .AnyAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);
        }

        // ===== QUẢN LÝ SECTIONS =====
        public async Task<List<Section>> GetCourseSectionsAsync(Guid courseId, string teacherId)
        {
            // Kiểm tra quyền sở hữu
            var owns = await IsTeacherOwnsCourseAsync(courseId, teacherId);
            if (!owns)
                return new List<Section>();

            return await _context.Sections
                .Include(s => s.Lessons.OrderBy(l => l.OrderIndex))
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync();
        }

        public async Task<Section?> GetSectionByIdAsync(int sectionId)
        {
            return await _context.Sections
                .Include(s => s.Lessons.OrderBy(l => l.OrderIndex))
                .FirstOrDefaultAsync(s => s.SectionId == sectionId);
        }

        public async Task<Section> CreateSectionAsync(Section section)
        {
            _context.Sections.Add(section);
            await _context.SaveChangesAsync();
            return section;
        }

        public async Task<bool> UpdateSectionAsync(Section section)
        {
            try
            {
                _context.Sections.Update(section);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteSectionAsync(int sectionId)
        {
            var section = await _context.Sections.FindAsync(sectionId);
            if (section == null)
                return false;

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();
            return true;
        }

        // ===== QUẢN LÝ LESSONS =====
        public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
        {
            return await _context.Lessons
                .Include(l => l.Section)
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);
        }

        public async Task<Lesson> CreateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<bool> UpdateLessonAsync(Lesson lesson)
        {
            try
            {
                _context.Lessons.Update(lesson);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteLessonAsync(int lessonId)
        {
            var lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null)
                return false;

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return true;
        }

        // ===== QUẢN LÝ QUIZ =====
        public async Task<Quiz?> GetQuizByLessonIdAsync(int lessonId)
        {
            return await _context.Quizzes
                .FirstOrDefaultAsync(q => q.LessonId == lessonId);
        }

        public async Task<List<Quiz>> GetQuizzesByLessonIdsAsync(List<int> lessonIds)
        {
            return await _context.Quizzes
                .Where(q => lessonIds.Contains(q.LessonId))
                .Select(q => new Quiz
                {
                    QuizId = q.QuizId,
                    LessonId = q.LessonId,
                    Title = q.Title
                })
                .ToListAsync();
        }

        public async Task<Quiz?> GetQuizWithDetailsAsync(int quizId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.QuizAnswers)
                .Include(q => q.Lesson)
                    .ThenInclude(l => l.Section)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);
        }

        public async Task<Quiz> CreateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<bool> UpdateQuizAsync(Quiz quiz)
        {
            try
            {
                _context.Quizzes.Update(quiz);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuizWithQuestionsAsync(Quiz quiz, List<Question> questionsToDelete, List<QuizAnswer> answersToDelete, List<Question> newQuestions)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Cập nhật title của quiz bằng raw SQL hoặc detach approach
                var quizToUpdate = await _context.Quizzes.FindAsync(quiz.QuizId);
                if (quizToUpdate == null) return false;
                quizToUpdate.Title = quiz.Title;

                // 2. Xóa template answers cũ trước (child records)
                if (answersToDelete.Any())
                {
                    var answerIds = answersToDelete.Select(a => a.AnswerId).ToList();
                    var answersInDb = await _context.QuizAnswers
                        .Where(a => answerIds.Contains(a.AnswerId))
                        .ToListAsync();
                    _context.QuizAnswers.RemoveRange(answersInDb);
                    await _context.SaveChangesAsync();
                }

                // 3. Xóa questions cũ (parent records)
                if (questionsToDelete.Any())
                {
                    var questionIds = questionsToDelete.Select(q => q.QuestionId).ToList();
                    var questionsInDb = await _context.Questions
                        .Where(q => questionIds.Contains(q.QuestionId))
                        .ToListAsync();
                    _context.Questions.RemoveRange(questionsInDb);
                    await _context.SaveChangesAsync();
                }

                // 4. Thêm questions mới với answers
                if (newQuestions.Any())
                {
                    foreach (var question in newQuestions)
                    {
                        question.QuizId = quiz.QuizId;
                    }
                    _context.Questions.AddRange(newQuestions);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log exception for debugging
                Console.WriteLine($"UpdateQuizWithQuestionsAsync Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteQuizAsync(int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.QuizAnswers)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);
            
            if (quiz == null)
                return false;

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
