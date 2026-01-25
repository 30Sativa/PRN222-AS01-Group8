using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class AiMessage
    {
        [Key]
        public Guid MessageId { get; set; }

        public Guid? ConversationId { get; set; }

        [MaxLength(10)]
        public string Role { get; set; } // user | ai

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ConversationId")]
        public virtual AiConversation AiConversation { get; set; }
    }
}
