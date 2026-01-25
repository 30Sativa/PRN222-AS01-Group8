namespace OnlineLearningPlatform.Services.DTO.Response
{
    public class PaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public string UserId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public Guid? EnrollmentId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
