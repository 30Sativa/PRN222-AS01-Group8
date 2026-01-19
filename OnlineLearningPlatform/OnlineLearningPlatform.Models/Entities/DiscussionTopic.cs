using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class DiscussionTopic
    {
        [Key]
        public Guid TopicId { get; set; }

        public Guid? CourseId { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<DiscussionReply> DiscussionReplies { get; set; }
    }
}
