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

        // ===== QUẢN LÝ SECTIONS =====
        public async Task<List<SectionDto>> GetCourseSectionsAsync(Guid courseId, string teacherId)
        {
            var sections = await _teacherRepository.GetCourseSectionsAsync(courseId, teacherId);
            var sectionDtos = new List<SectionDto>();

            foreach (var section in sections)
            {
                var sectionDto = new SectionDto
                {
                    SectionId = section.SectionId,
                    CourseId = section.CourseId,
                    Title = section.Title,
                    OrderIndex = section.OrderIndex,
                    TotalLessons = section.Lessons?.Count ?? 0
                };

                if (section.Lessons != null)
                {
                    sectionDto.Lessons = section.Lessons.Select(l => new LessonDto
                    {
                        LessonId = l.LessonId,
                        SectionId = l.SectionId,
                        Title = l.Title,
                        LessonType = l.LessonType,
                        Content = l.Content,
                        OrderIndex = l.OrderIndex
                    }).ToList();
                }

                sectionDtos.Add(sectionDto);
            }

            return sectionDtos;
        }

        public async Task<SectionDto?> GetSectionByIdAsync(int sectionId)
        {
            var section = await _teacherRepository.GetSectionByIdAsync(sectionId);
            if (section == null)
                return null;

            var sectionDto = new SectionDto
            {
                SectionId = section.SectionId,
                CourseId = section.CourseId,
                Title = section.Title,
                OrderIndex = section.OrderIndex,
                TotalLessons = section.Lessons?.Count ?? 0
            };

            if (section.Lessons != null)
            {
                sectionDto.Lessons = section.Lessons.Select(l => new LessonDto
                {
                    LessonId = l.LessonId,
                    SectionId = l.SectionId,
                    Title = l.Title,
                    LessonType = l.LessonType,
                    Content = l.Content,
                    OrderIndex = l.OrderIndex
                }).ToList();
            }

            return sectionDto;
        }

        public async Task<int> CreateSectionAsync(Guid courseId, CreateSectionRequest request, string teacherId)
        {
            // Kiểm tra quyền sở hữu khóa học
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(courseId, teacherId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Bạn không có quyền thêm chương vào khóa học này");

            var section = new Section
            {
                CourseId = courseId,
                Title = request.Title,
                OrderIndex = request.OrderIndex
            };

            var createdSection = await _teacherRepository.CreateSectionAsync(section);
            return createdSection.SectionId;
        }

        public async Task<bool> UpdateSectionAsync(int sectionId, UpdateSectionRequest request, string teacherId)
        {
            var section = await _teacherRepository.GetSectionByIdAsync(sectionId);
            if (section == null)
                return false;

            // Kiểm tra quyền sở hữu khóa học
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(section.CourseId, teacherId);
            if (!isOwner)
                return false;

            section.Title = request.Title;
            section.OrderIndex = request.OrderIndex;

            return await _teacherRepository.UpdateSectionAsync(section);
        }

        public async Task<bool> DeleteSectionAsync(int sectionId, string teacherId)
        {
            var section = await _teacherRepository.GetSectionByIdAsync(sectionId);
            if (section == null)
                return false;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(section.CourseId, teacherId);
            if (!isOwner)
                return false;

            return await _teacherRepository.DeleteSectionAsync(sectionId);
        }

        // ===== QUẢN LÝ LESSONS =====
        public async Task<LessonDto?> GetLessonByIdAsync(int lessonId)
        {
            var lesson = await _teacherRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null)
                return null;

            return new LessonDto
            {
                LessonId = lesson.LessonId,
                SectionId = lesson.SectionId,
                Title = lesson.Title,
                LessonType = lesson.LessonType,
                Content = lesson.Content,
                OrderIndex = lesson.OrderIndex
            };
        }

        public async Task<int> CreateLessonAsync(int sectionId, CreateLessonRequest request, string teacherId)
        {
            var section = await _teacherRepository.GetSectionByIdAsync(sectionId);
            if (section == null)
                throw new ArgumentException("Không tìm thấy chương");

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(section.CourseId, teacherId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Bạn không có quyền thêm bài học vào khóa học này");

            var lesson = new Lesson
            {
                SectionId = sectionId,
                Title = request.Title,
                LessonType = request.LessonType,
                Content = request.Content,
                OrderIndex = request.OrderIndex
            };

            var createdLesson = await _teacherRepository.CreateLessonAsync(lesson);
            return createdLesson.LessonId;
        }

        public async Task<bool> UpdateLessonAsync(int lessonId, UpdateLessonRequest request, string teacherId)
        {
            var lesson = await _teacherRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null || lesson.Section == null)
                return false;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(lesson.Section.CourseId, teacherId);
            if (!isOwner)
                return false;

            lesson.Title = request.Title;
            lesson.LessonType = request.LessonType;
            lesson.Content = request.Content;
            lesson.OrderIndex = request.OrderIndex;

            return await _teacherRepository.UpdateLessonAsync(lesson);
        }

        public async Task<bool> DeleteLessonAsync(int lessonId, string teacherId)
        {
            var lesson = await _teacherRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null || lesson.Section == null)
                return false;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(lesson.Section.CourseId, teacherId);
            if (!isOwner)
                return false;

            return await _teacherRepository.DeleteLessonAsync(lessonId);
        }
    }
}
