using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.DTO.Response;
using OnlineLearningPlatform.Services.DTO.Response.Teacher;
using OnlineLearningPlatform.Services.DTO.Teacher;

namespace OnlineLearningPlatform.Services.Interfaces
{
    /// <summary>
    /// Service riêng cho Teacher - Tách biệt khỏi ICourseService để tránh conflict
    /// </summary>
    public interface ITeacherService
    {
        // Quản lý danh sách khóa học
        Task<List<TeacherCourseDto>> GetTeacherCoursesAsync(string teacherId);

        // Lấy chi tiết khóa học để chỉnh sửa
        Task<TeacherCourseDto?> GetTeacherCourseByIdAsync(Guid courseId, string teacherId);

        // Lấy danh sách categories
        Task<List<CategoryDto>> GetCategoriesAsync();

        // Tạo khóa học mới
        Task<Guid> CreateCourseAsync(CreateCourseRequest request, string teacherId);

        // Cập nhật khóa học
        Task<bool> UpdateCourseAsync(Guid courseId, UpdateCourseRequest request, string teacherId);

        // Xóa khóa học
        Task<bool> DeleteCourseAsync(Guid courseId, string teacherId);

        // ===== QUẢN LÝ SECTIONS =====
        // Lấy danh sách sections và lessons của khóa học
        Task<List<TeacherSectionDto>> GetCourseSectionsAsync(Guid courseId, string teacherId);

        // Lấy chi tiết section
        Task<TeacherSectionDto?> GetSectionByIdAsync(int sectionId);

        // Tạo section mới
        Task<int> CreateSectionAsync(Guid courseId, CreateSectionRequest request, string teacherId);

        // Cập nhật section
        Task<bool> UpdateSectionAsync(int sectionId, UpdateSectionRequest request, string teacherId);

        // Xóa section
        Task<bool> DeleteSectionAsync(int sectionId, string teacherId);

        // ===== QUẢN LÝ LESSONS =====
        // Lấy chi tiết lesson
        Task<TeacherLessonDto?> GetLessonByIdAsync(int lessonId);

        // Tạo lesson mới
        Task<int> CreateLessonAsync(int sectionId, CreateLessonRequest request, string teacherId);

        // Cập nhật lesson
        Task<bool> UpdateLessonAsync(int lessonId, UpdateLessonRequest request, string teacherId);

        // Xóa lesson
        Task<bool> DeleteLessonAsync(int lessonId, string teacherId);
        
        // ===== QUẢN LÝ HỌC VIÊN =====
        /// <summary>
        /// Lấy danh sách học viên đã đăng ký khóa học
        /// </summary>
        Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, string teacherId);
        
        /// <summary>
        /// Xem chi tiết tiến độ học tập của 1 học viên
        /// </summary>
        Task<StudentProgressDto?> GetStudentProgressAsync(Guid courseId, string studentId, string teacherId);
    }
}
