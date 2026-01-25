using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class LessonProgress
    {
        [Key]
        public Guid ProgressId { get; set; }

        public string UserId { get; set; }

        public int? LessonId { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
    }
}
