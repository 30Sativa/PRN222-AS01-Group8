using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.DTO.Response;

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
    }
}
