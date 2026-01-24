using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Models.Enums;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Request.Course;
using OnlineLearningPlatform.Services.DTO.Response;
using OnlineLearningPlatform.Services.DTO.Response.Course;
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

        public async Task<List<CourseDto>> GetUserEnrolledCoursesAsync(string userId)
        {
            var enrollments = await _enrollmentRepository.GetUserEnrollmentsAsync(userId);
            var coursesDto = new List<CourseDto>();

            foreach (var enrollment in enrollments)
            {
                coursesDto.Add(new CourseDto
                {
                    CourseId = enrollment.Course.CourseId,
                    Title = enrollment.Course.Title,
                    Description = enrollment.Course.Description ?? string.Empty,
                    TeacherName = enrollment.Course.Teacher?.FullName ?? "Unknown",
                    CategoryName = enrollment.Course.Category?.CategoryName,
                    Price = enrollment.Course.Price,
                    CreatedAt = enrollment.Course.CreatedAt,
                    IsEnrolled = true
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


        //new methods for admin course management
        public async Task<List<CourseManagerDto>> GetCoursesAsync(CourseStatus? status, string? keyword)
        {
            var courses = await _courseRepository.GetAllAsync();

            if (status != null)
                courses = courses.Where(x => x.Status == status).ToList();

            if (!string.IsNullOrEmpty(keyword))
                courses = courses
                    .Where(x => x.Title.Contains(keyword))
                    .ToList();

            return courses.Select(x => new CourseManagerDto
            {
                CourseId = x.CourseId,
                Title = x.Title,
                TeacherName = x.Teacher.UserName,
                CategoryName = x.Category.CategoryName,
                Price = x.Price,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                RejectReason = x.RejectionReason
            }).ToList();
        }

        public async Task ApproveAsync(ApproveCourseRequest request)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);

            course!.Status = CourseStatus.Published;
            course.RejectionReason = null;

            await _courseRepository.SaveChangesAsync();
        }

        public async Task RejectAsync(RejectCourseRequest request)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);

            course!.Status = CourseStatus.Rejected;
            course.RejectionReason = request.Reason;

            await _courseRepository.SaveChangesAsync();
        }
    }
}
