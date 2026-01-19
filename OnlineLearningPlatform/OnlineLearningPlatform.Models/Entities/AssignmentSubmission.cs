using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class AssignmentSubmission
    {
        [Key]
        public Guid SubmissionId { get; set; }

        public int? AssignmentId { get; set; }

        public string UserId { get; set; }

        [MaxLength(255)]
        public string FileUrl { get; set; }

        public double? Score { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("AssignmentId")]
        public virtual Assignment Assignment { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
