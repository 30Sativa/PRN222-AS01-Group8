using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class AiConversation
    {
        [Key]
        public Guid ConversationId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<AiMessage> AiMessages { get; set; }
    }
}
