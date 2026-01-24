using OnlineLearningPlatform.Services.DTO.Teacher;

namespace OnlineLearningPlatform.Services.DTO.Teacher
{
    /// <summary>
    /// DTO for Teacher to manage sections (CRUD operations)
    /// Separate from student view to avoid conflicts
    /// </summary>
    public class TeacherSectionDto
    {
        public int SectionId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? OrderIndex { get; set; }
        public int TotalLessons { get; set; }
        public List<TeacherLessonDto> Lessons { get; set; } = new();
    }
}
