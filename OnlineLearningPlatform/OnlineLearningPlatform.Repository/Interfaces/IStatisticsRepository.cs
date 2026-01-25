using OnlineLearningPlatform.Models.Identity;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    /// <summary>
    /// Repository lấy dữ liệu thống kê cho dashboard admin.
    /// </summary>
    public interface IStatisticsRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalCoursesAsync();
        Task<int> GetTotalEnrollmentsAsync();
        /// <summary>Số tháng cần lấy (ví dụ 8).</summary>
        Task<List<(string Label, decimal Value)>> GetRevenueByMonthAsync(int lastMonths = 8);
        /// <summary>Giảng viên có nhiều khóa học nhất (trả về null nếu không có).</summary>
        Task<(ApplicationUser? Teacher, int CourseCount)> GetTopTeacherByCourseCountAsync();
    }
}
