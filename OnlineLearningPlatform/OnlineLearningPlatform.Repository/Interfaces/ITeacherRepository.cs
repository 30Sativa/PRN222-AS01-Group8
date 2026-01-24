using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    /// <summary>
    /// Repository riêng cho Teacher - Quản lý khóa học của giảng viên
    /// </summary>
    public interface ITeacherRepository
    {
        // Lấy danh sách khóa học của teacher
        Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId);

        // Lấy chi tiết khóa học với thống kê
        Task<Course?> GetCourseWithStatisticsAsync(Guid courseId, string teacherId);

        // Tạo khóa học mới
        Task<Course> CreateCourseAsync(Course course);

        // Cập nhật khóa học
        Task<bool> UpdateCourseAsync(Course course);

        // Xóa khóa học
        Task<bool> DeleteCourseAsync(Guid courseId, string teacherId);

        // Kiểm tra quyền sở hữu khóa học
        Task<bool> IsTeacherOwnsCourseAsync(Guid courseId, string teacherId);
    }
}
