using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        public Guid? EnrollmentId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Required]
        public decimal Amount { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // pending | success | failed

        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // demo | credit_card | bank_transfer | etc.

        [MaxLength(200)]
        public string? TransactionId { get; set; } // ID từ payment gateway (demo)

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [ForeignKey("EnrollmentId")]
        public virtual Enrollment? Enrollment { get; set; }
    }
}
