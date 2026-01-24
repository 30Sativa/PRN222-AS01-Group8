namespace OnlineLearningPlatform.Services.DTO.Response.Course
{
    public class CourseDetailDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string TeacherName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEnrolled { get; set; }
        public List<SectionDto> Sections { get; set; } = new();
    }

    public class SectionDto
    {
        public int SectionId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int? OrderIndex { get; set; }
        public int TotalLessons { get; set; }
        public List<LessonDto> Lessons { get; set; } = new();
    }

    public class LessonDto
    {
        public int LessonId { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = null!;
        public string LessonType { get; set; } = null!;
        public string Content { get; set; } = null!; // Video URL hoặc content
        public int? OrderIndex { get; set; }
        // Các properties bổ sung cho WatchLesson view
        public string? CourseTitle { get; set; }
        public Guid? CourseId { get; set; }
        public string? SectionTitle { get; set; }
    }
}
