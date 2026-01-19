using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class QuizAttempt
    {
        [Key]
        public Guid AttemptId { get; set; }

        public int? QuizId { get; set; }

        public string UserId { get; set; }

        public double? Score { get; set; }

        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("QuizId")]
        public virtual Quiz Quiz { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<QuizAnswer> QuizAnswers { get; set; }
    }
}
