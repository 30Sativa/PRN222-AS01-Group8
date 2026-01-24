namespace OnlineLearningPlatform.Services.DTO.Teacher
{
    /// <summary>
    /// DTO for Teacher to manage lessons (CRUD operations)
    /// Separate from student view to avoid conflicts
    /// </summary>
    public class TeacherLessonDto
    {
        public int LessonId { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string LessonType { get; set; } = string.Empty; // video | reading | quiz | assignment
        public string Content { get; set; } = string.Empty;
        public int? OrderIndex { get; set; }

        // Optional for breadcrumb navigation
        public string? CourseTitle { get; set; }
        public Guid? CourseId { get; set; }
        public string? SectionTitle { get; set; }
    }
}
