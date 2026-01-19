using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Section
    {
        [Key]
        public int SectionId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public int? OrderIndex { get; set; }

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
