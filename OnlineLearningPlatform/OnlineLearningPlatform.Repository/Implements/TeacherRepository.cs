using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
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
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Enrollments)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .Where(c => c.TeacherId == teacherId)
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
    }
}
