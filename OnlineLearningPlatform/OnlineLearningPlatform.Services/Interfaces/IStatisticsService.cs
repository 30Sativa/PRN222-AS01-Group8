using OnlineLearningPlatform.Services.DTO.Response.Admin;

namespace OnlineLearningPlatform.Services.Interfaces
{
    /// <summary>
    /// Service thống kê tổng quan cho trang admin.
    /// </summary>
    public interface IStatisticsService
    {
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
    }
}
