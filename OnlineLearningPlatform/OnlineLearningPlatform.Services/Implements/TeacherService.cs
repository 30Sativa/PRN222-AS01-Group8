using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.DTO.Response;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    /// <summary>
    /// Service implementation cho Teacher - Tách riêng để tránh conflict với team
    /// </summary>
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TeacherService(
            ITeacherRepository teacherRepository,
            ICategoryRepository categoryRepository)
        {
            _teacherRepository = teacherRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<TeacherCourseDto>> GetTeacherCoursesAsync(string teacherId)
        {
            var courses = await _teacherRepository.GetCoursesByTeacherIdAsync(teacherId);
            var teacherCoursesDto = new List<TeacherCourseDto>();

            foreach (var course in courses)
            {
                teacherCoursesDto.Add(new TeacherCourseDto
                {
                    CourseId = course.CourseId,
                    Title = course.Title,
                    Description = course.Description ?? string.Empty,
                    CategoryName = course.Category?.CategoryName,
                    Price = course.Price,
                    CreatedAt = course.CreatedAt,
                    TotalEnrollments = course.Enrollments?.Count ?? 0,
                    TotalSections = course.Sections?.Count ?? 0,
                    TotalLessons = course.Sections?.Sum(s => s.Lessons?.Count ?? 0) ?? 0
                });
            }

            return teacherCoursesDto;
        }

        public async Task<TeacherCourseDto?> GetTeacherCourseByIdAsync(Guid courseId, string teacherId)
        {
            var course = await _teacherRepository.GetCourseWithStatisticsAsync(courseId, teacherId);

            if (course == null)
                return null;

            return new TeacherCourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description ?? string.Empty,
                CategoryName = course.Category?.CategoryName,
                Price = course.Price,
                CreatedAt = course.CreatedAt,
                TotalEnrollments = course.Enrollments?.Count ?? 0,
                TotalSections = course.Sections?.Count ?? 0,
                TotalLessons = course.Sections?.Sum(s => s.Lessons?.Count ?? 0) ?? 0
            };
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
        }

        public async Task<Guid> CreateCourseAsync(CreateCourseRequest request, string teacherId)
        {
            var course = new Course
            {
                CourseId = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                TeacherId = teacherId,
                CreatedAt = DateTime.UtcNow
            };

            var createdCourse = await _teacherRepository.CreateCourseAsync(course);
            return createdCourse.CourseId;
        }

        public async Task<bool> UpdateCourseAsync(Guid courseId, UpdateCourseRequest request, string teacherId)
        {
            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(courseId, teacherId);
            if (!isOwner)
                return false;

            var course = await _teacherRepository.GetCourseWithStatisticsAsync(courseId, teacherId);
            if (course == null)
                return false;

            // Cập nhật thông tin
            course.Title = request.Title;
            course.Description = request.Description;
            course.Price = request.Price;
            course.CategoryId = request.CategoryId;

            return await _teacherRepository.UpdateCourseAsync(course);
        }

        public async Task<bool> DeleteCourseAsync(Guid courseId, string teacherId)
        {
            return await _teacherRepository.DeleteCourseAsync(courseId, teacherId);
        }
    }
}
