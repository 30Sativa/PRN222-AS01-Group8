using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class QuizAnswer
    {
        [Key]
        public Guid AnswerId { get; set; }

        public Guid? AttemptId { get; set; }

        public int? QuestionId { get; set; }

        [MaxLength(200)]
        public string UserAnswer { get; set; }

        // Navigation properties
        [ForeignKey("AttemptId")]
        public virtual QuizAttempt QuizAttempt { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
