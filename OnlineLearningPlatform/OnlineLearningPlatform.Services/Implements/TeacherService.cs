using OnlineLearningPlatform.Repositories.Interfaces;
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

        public TeacherService(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
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
    }
}
