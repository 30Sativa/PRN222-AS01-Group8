namespace OnlineLearningPlatform.Services.DTO.Response.Teacher
{
    /// <summary>
    /// DTO cho thông tin học viên đã đăng ký khóa học
    /// </summary>
    public class EnrollmentDto
    {
        public Guid EnrollmentId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
    }
}
