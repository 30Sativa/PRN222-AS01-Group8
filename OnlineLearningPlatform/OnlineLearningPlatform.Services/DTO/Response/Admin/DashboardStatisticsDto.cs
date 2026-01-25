namespace OnlineLearningPlatform.Services.DTO.Response.Admin
{
    /// <summary>
    /// DTO thống kê tổng quan cho trang admin.
    /// </summary>
    public class DashboardStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public string? TopTeacherName { get; set; }
        public int TopTeacherCourseCount { get; set; }
        /// <summary>Doanh thu theo tháng (Label, Value) cho biểu đồ, 8 tháng gần nhất.</summary>
        public List<RevenueByMonthItem> RevenueByMonth { get; set; } = new();
    }

    public class RevenueByMonthItem
    {
        public string Label { get; set; } = "";
        public decimal Value { get; set; }
    }
}
