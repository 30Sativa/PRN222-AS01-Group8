using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(20)]
        public string LessonType { get; set; } // video | reading | quiz | assignment

        public string Content { get; set; }

        public int? OrderIndex { get; set; }

        // Navigation properties
        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }

        public virtual ICollection<LessonProgress> LessonProgresses { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
