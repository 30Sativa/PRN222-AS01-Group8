using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Response;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public CourseService(
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<List<CourseDto>> GetAllCoursesAsync(string? userId = null)
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            var coursesDto = new List<CourseDto>();

            foreach (var course in courses)
            {
                var isEnrolled = false;
                if (!string.IsNullOrEmpty(userId))
                {
                    isEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(userId, course.CourseId);
                }

                coursesDto.Add(new CourseDto
                {
                    CourseId = course.CourseId,
                    Title = course.Title,
                    Description = course.Description ?? string.Empty,
                    TeacherName = course.Teacher?.FullName ?? "Unknown",
                    CategoryName = course.Category?.CategoryName,
                    Price = course.Price,
                    CreatedAt = course.CreatedAt,
                    IsEnrolled = isEnrolled
                });
            }

            return coursesDto;
        }

        public async Task<CourseDetailDto?> GetCourseDetailAsync(Guid courseId, string? userId = null)
        {
            var course = await _courseRepository.GetCourseWithDetailsAsync(courseId);
            if (course == null) return null;

            var isEnrolled = false;
            if (!string.IsNullOrEmpty(userId))
            {
                isEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId);
            }

            var sectionsDto = new List<SectionDto>();
            foreach (var section in course.Sections.OrderBy(s => s.OrderIndex))
            {
                var lessonsDto = section.Lessons
                    .OrderBy(l => l.OrderIndex)
                    .Select(l => new LessonDto
                    {
                        LessonId = l.LessonId,
                        Title = l.Title,
                        LessonType = l.LessonType,
                        OrderIndex = l.OrderIndex
                    }).ToList();

                sectionsDto.Add(new SectionDto
                {
                    SectionId = section.SectionId,
                    Title = section.Title,
                    OrderIndex = section.OrderIndex,
                    Lessons = lessonsDto
                });
            }

            return new CourseDetailDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description ?? string.Empty,
                TeacherName = course.Teacher?.FullName ?? "Unknown",
                CategoryName = course.Category?.CategoryName,
                Price = course.Price,
                CreatedAt = course.CreatedAt,
                IsEnrolled = isEnrolled,
                Sections = sectionsDto
            };
        }
    }
}
