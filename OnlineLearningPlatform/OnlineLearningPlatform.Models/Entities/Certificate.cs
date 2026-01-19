using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Certificate
    {
        [Key]
        public Guid CertificateId { get; set; }

        [Required]
        public string UserId { get; set; }

        public Guid? CourseId { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
