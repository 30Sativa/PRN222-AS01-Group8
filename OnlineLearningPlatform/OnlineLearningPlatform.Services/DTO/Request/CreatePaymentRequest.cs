using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Services.DTO.Request
{
    public class CreatePaymentRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
    }
}
