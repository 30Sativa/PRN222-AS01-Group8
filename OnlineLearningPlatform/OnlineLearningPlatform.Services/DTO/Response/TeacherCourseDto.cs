namespace OnlineLearningPlatform.Services.DTO.Response
{
    public class TeacherCourseDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }

        // Statistics
        public int TotalEnrollments { get; set; }
        public int TotalSections { get; set; }
        public int TotalLessons { get; set; }
    }
}
