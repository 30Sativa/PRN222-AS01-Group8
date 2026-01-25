using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }

        public Guid? CourseId { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
