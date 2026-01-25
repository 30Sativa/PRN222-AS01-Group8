using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Response.Admin;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statsRepo;

        public StatisticsService(IStatisticsRepository statsRepo)
        {
            _statsRepo = statsRepo;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            // DbContext không hỗ trợ nhiều truy vấn đồng thời trên cùng instance → gọi tuần tự.
            var totalUsers = await _statsRepo.GetTotalUsersAsync();
            var totalCourses = await _statsRepo.GetTotalCoursesAsync();
            var totalEnrollments = await _statsRepo.GetTotalEnrollmentsAsync();
            var revenueList = await _statsRepo.GetRevenueByMonthAsync(8);
            var (teacher, courseCount) = await _statsRepo.GetTopTeacherByCourseCountAsync();

            return new DashboardStatisticsDto
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                RevenueByMonth = revenueList
                    .Select(x => new RevenueByMonthItem { Label = x.Label, Value = x.Value })
                    .ToList(),
                TopTeacherName = teacher?.FullName,
                TopTeacherCourseCount = courseCount
            };
        }
    }
}
