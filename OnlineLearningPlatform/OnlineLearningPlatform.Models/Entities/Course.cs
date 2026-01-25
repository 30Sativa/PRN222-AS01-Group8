using OnlineLearningPlatform.Models.Enums;
using OnlineLearningPlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Course
    {
        [Key]
        public Guid CourseId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string TeacherId { get; set; }

        public int? CategoryId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; } = 0;
        public CourseStatus Status { get; set; } = CourseStatus.Pending;
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("TeacherId")]
        public virtual ApplicationUser Teacher { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<DiscussionTopic> DiscussionTopics { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<Announcement> Announcements { get; set; }
    }
}
