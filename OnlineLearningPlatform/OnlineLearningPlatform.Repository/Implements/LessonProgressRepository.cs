using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class LessonProgressRepository : ILessonProgressRepository
    {
        private readonly ApplicationDbContext _context;

        public LessonProgressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LessonProgress>> GetStudentProgressInCourseAsync(string studentId, Guid courseId)
        {
            return await _context.LessonProgresses
                .Include(lp => lp.Lesson)
                    .ThenInclude(l => l.Section)
                .Where(lp => lp.UserId == studentId && lp.Lesson.Section.CourseId == courseId)
                .OrderBy(lp => lp.Lesson.Section.OrderIndex)
                    .ThenBy(lp => lp.Lesson.OrderIndex)
                .ToListAsync();
        }

        public async Task<LessonProgress?> GetLessonProgressAsync(string studentId, int lessonId)
        {
            return await _context.LessonProgresses
                .Include(lp => lp.Lesson)
                .FirstOrDefaultAsync(lp => lp.UserId == studentId && lp.LessonId == lessonId);
        }
    }
}
