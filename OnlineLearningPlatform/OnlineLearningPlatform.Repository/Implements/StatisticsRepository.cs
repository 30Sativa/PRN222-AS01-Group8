using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public StatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalCoursesAsync()
        {
            return await _context.Courses.CountAsync();
        }

        public async Task<int> GetTotalEnrollmentsAsync()
        {
            return await _context.Enrollments.CountAsync();
        }

        public async Task<List<(string Label, decimal Value)>> GetRevenueByMonthAsync(int lastMonths = 8)
        {
            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-lastMonths);
            var list = await _context.Payments
                .Where(p => p.PaymentDate >= start && p.Status == "success")
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(p => p.Amount) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var result = new List<(string Label, decimal Value)>();
            for (var d = start; d <= now; d = d.AddMonths(1))
            {
                var item = list.FirstOrDefault(x => x.Year == d.Year && x.Month == d.Month);
                var label = d.ToString("M/y", System.Globalization.CultureInfo.GetCultureInfo("vi-VN"));
                result.Add((label, item?.Total ?? 0));
            }
            return result;
        }

        public async Task<(ApplicationUser? Teacher, int CourseCount)> GetTopTeacherByCourseCountAsync()
        {
            var top = await _context.Courses
                .Where(c => c.TeacherId != null)
                .GroupBy(c => c.TeacherId)
                .OrderByDescending(g => g.Count())
                .Select(g => new { TeacherId = g.Key!, Count = g.Count() })
                .FirstOrDefaultAsync();

            if (top == null)
                return (null, 0);

            var teacher = await _context.Users.FindAsync(top.TeacherId);
            return (teacher, top.Count);
        }
    }
}
