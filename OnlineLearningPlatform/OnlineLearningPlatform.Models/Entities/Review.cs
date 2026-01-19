using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public Guid? CourseId { get; set; }

        public string UserId { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }

        public string Comment { get; set; }

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
