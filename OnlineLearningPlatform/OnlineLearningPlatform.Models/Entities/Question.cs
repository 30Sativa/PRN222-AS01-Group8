using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        public int? QuizId { get; set; }

        public string Content { get; set; }

        [MaxLength(20)]
        public string QuestionType { get; set; } // mcq | truefalse | text

        [MaxLength(200)]
        public string CorrectAnswer { get; set; }

        // Navigation properties
        [ForeignKey("QuizId")]
        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<QuizAnswer> QuizAnswers { get; set; }
    }
}
