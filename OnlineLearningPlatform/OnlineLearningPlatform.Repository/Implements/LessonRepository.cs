using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class LessonRepository : ILessonRepository
    {
        private readonly ApplicationDbContext _context;

        public LessonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
        {
            return await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);
        }

        public async Task<bool> IsUserEnrolledInLessonCourseAsync(string userId, int lessonId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);

            if (lesson?.Section?.Course == null)
            {
                return false;
            }

            var courseId = lesson.Section.Course.CourseId;
            return await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task<LessonProgress?> GetLessonProgressAsync(string userId, int lessonId)
        {
            return await _context.LessonProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);
        }

        public async Task MarkLessonCompleteAsync(string userId, int lessonId)
        {
            var progress = await _context.LessonProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

            if (progress == null)
            {
                progress = new LessonProgress
                {
                    ProgressId = Guid.NewGuid(),
                    UserId = userId,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };
                await _context.LessonProgresses.AddAsync(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                _context.LessonProgresses.Update(progress);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetCompletedLessonIdsAsync(string userId, Guid courseId)
        {
            return await _context.LessonProgresses
                .Where(p => p.UserId == userId && 
                           p.IsCompleted && 
                           p.Lesson.Section.CourseId == courseId)
                .Select(p => p.LessonId ?? 0)
                .ToListAsync();
        }
    }
}
