namespace OnlineLearningPlatform.Services.DTO.Response.Course
{
    public class CourseDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string TeacherName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEnrolled { get; set; }
    }
}
