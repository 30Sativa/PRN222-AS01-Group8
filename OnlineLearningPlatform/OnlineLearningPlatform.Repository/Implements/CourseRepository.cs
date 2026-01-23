using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(Guid courseId)
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<Course?> GetCourseWithDetailsAsync(Guid courseId)
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderIndex))
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<bool> CourseExistsAsync(Guid courseId)
        {
            return await _context.Courses.AnyAsync(c => c.CourseId == courseId);
        }
    }
}
