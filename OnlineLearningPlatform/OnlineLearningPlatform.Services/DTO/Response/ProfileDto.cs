namespace OnlineLearningPlatform.Services.DTO.Response
{
    public class ProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int CourseCount { get; set; }
        public int ArticleCount { get; set; }
    }
}
