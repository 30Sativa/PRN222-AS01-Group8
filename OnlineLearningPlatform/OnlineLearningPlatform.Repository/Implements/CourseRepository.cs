using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Models.Enums;
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
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<bool> CourseExistsAsync(Guid courseId)
        {
            return await _context.Courses.AnyAsync(c => c.CourseId == courseId);
        }

        public async Task<List<Course>> SearchCoursesAsync(string? keyword)
        {
            var query = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(c => 
                    c.Title.ToLower().Contains(keyword) || 
                    (c.Description != null && c.Description.ToLower().Contains(keyword)));
            }

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // new methods to be implemented
    

        public Task<Course?> GetByIdAsync(Guid id)
        {
            return _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
        }

      
        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Courses
                                         .Include(x => x.Teacher)
                                         .Include(x => x.Category)
                                         .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
