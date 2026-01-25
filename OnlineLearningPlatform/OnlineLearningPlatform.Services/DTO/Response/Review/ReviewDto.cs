namespace OnlineLearningPlatform.Services.DTO.Response.Review
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool CanEdit { get; set; } // User có thể chỉnh sửa review này không
    }
}
