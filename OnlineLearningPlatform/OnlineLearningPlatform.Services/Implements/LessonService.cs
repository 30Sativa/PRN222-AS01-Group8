using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Response.Course;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonService(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<LessonDto?> GetLessonDetailAsync(int lessonId, string userId)
        {
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null)
            {
                return null;
            }

            // Kiểm tra user đã đăng ký course chưa
            var isEnrolled = await _lessonRepository.IsUserEnrolledInLessonCourseAsync(userId, lessonId);
            if (!isEnrolled)
            {
                return null; // Không cho xem nếu chưa đăng ký
            }

            return new LessonDto
            {
                LessonId = lesson.LessonId,
                Title = lesson.Title ?? string.Empty,
                LessonType = lesson.LessonType ?? string.Empty,
                Content = lesson.Content ?? string.Empty,
                OrderIndex = lesson.OrderIndex,
                CourseTitle = lesson.Section?.Course?.Title ?? "Unknown Course",
                CourseId = lesson.Section?.Course?.CourseId,
                SectionTitle = lesson.Section?.Title ?? "Unknown Section"
            };
        }
    }
}
