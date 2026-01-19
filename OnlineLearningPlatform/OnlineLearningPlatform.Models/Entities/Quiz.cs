using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public int LessonId { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        // Navigation properties
        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; }
    }
}
