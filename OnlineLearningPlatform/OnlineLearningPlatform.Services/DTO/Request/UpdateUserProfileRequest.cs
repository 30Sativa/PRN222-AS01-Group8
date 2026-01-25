namespace OnlineLearningPlatform.Services.DTO.Request
{
    public class UpdateUserProfileRequest
    {
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }
}
