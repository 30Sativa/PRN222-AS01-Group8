using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }

        public string UserId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } // pending | success | failed

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
