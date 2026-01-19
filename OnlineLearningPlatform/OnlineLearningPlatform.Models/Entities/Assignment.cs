using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int? LessonId { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        // Navigation properties
        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }

        public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; }
    }
}
