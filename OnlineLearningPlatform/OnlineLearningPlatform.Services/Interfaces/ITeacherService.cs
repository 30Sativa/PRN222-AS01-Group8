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

        /// <summary>Lấy danh sách khóa học đang chờ duyệt của giáo viên.</summary>
        Task<List<TeacherCourseDto>> GetTeacherPendingCoursesAsync(string teacherId);

        // Lấy chi tiết khóa học để chỉnh sửa
        Task<TeacherCourseDto?> GetTeacherCourseByIdAsync(Guid courseId, string teacherId);

        // Lấy danh sách categories
        Task<List<CategoryDto>> GetCategoriesAsync();

        // ===== QUẢN LÝ DANH MỤC KHÓA HỌC =====
        /// <summary>Lấy danh sách danh mục (dùng cho trang quản lý danh mục).</summary>
        Task<List<CategoryDto>> GetCategoriesForManagementAsync();
        /// <summary>Tạo danh mục mới. Trả về CategoryId nếu thành công.</summary>
        Task<int> CreateCategoryAsync(string categoryName);
        /// <summary>Cập nhật danh mục. Trả về true nếu thành công.</summary>
        Task<bool> UpdateCategoryAsync(int categoryId, string categoryName);
        /// <summary>Xóa danh mục. Trả về (success, message).</summary>
        Task<(bool Success, string Message)> DeleteCategoryAsync(int categoryId);

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

        // ===== QUẢN LÝ QUIZ =====
        /// <summary>
        /// Kiểm tra xem lesson đã có quiz chưa
        /// </summary>
        Task<bool> HasQuizForLessonAsync(int lessonId);

        /// <summary>
        /// Lấy danh sách quiz theo danh sách lessonIds (dùng cho ManageSections)
        /// </summary>
        Task<Dictionary<int, (int QuizId, string Title)>> GetQuizzesByLessonIdsAsync(List<int> lessonIds);

        /// <summary>
        /// Tạo quiz mới cho lesson
        /// </summary>
        Task<int> CreateQuizAsync(int lessonId, CreateQuizRequest request, string teacherId);

        /// <summary>
        /// Lấy chi tiết quiz với questions và answers
        /// </summary>
        Task<QuizDetailDto?> GetQuizDetailsAsync(int quizId, string teacherId);

        /// <summary>
        /// Cập nhật quiz
        /// </summary>
        Task<bool> UpdateQuizAsync(int quizId, UpdateQuizRequest request, string teacherId);

        /// <summary>
        /// Xóa quiz
        /// </summary>
        Task<bool> DeleteQuizAsync(int quizId, string teacherId);
    }
}
