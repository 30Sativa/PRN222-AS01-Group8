namespace OnlineLearningPlatform.Services.DTO.Response
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
        public string Title { get; set; } = null!;
        public int? OrderIndex { get; set; }
        public List<LessonDto> Lessons { get; set; } = new();
    }

    public class LessonDto
    {
        public int LessonId { get; set; }
        public string Title { get; set; } = null!;
        public string LessonType { get; set; } = null!;
        public int? OrderIndex { get; set; }
    }
}
