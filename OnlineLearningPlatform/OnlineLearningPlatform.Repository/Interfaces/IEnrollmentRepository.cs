using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<bool> IsUserEnrolledAsync(string userId, Guid courseId);
        Task<List<Enrollment>> GetUserEnrollmentsAsync(string userId);
        Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
        
        /// <summary>
        /// Lấy danh sách học viên đã đăng ký khóa học (cho Teacher)
        /// </summary>
        Task<List<Enrollment>> GetCourseEnrollmentsAsync(Guid courseId);
        
        /// <summary>
        /// Lấy enrollment cụ thể của 1 học viên trong 1 khóa
        /// </summary>
        Task<Enrollment?> GetEnrollmentByStudentAndCourseAsync(string studentId, Guid courseId);
    }
}
