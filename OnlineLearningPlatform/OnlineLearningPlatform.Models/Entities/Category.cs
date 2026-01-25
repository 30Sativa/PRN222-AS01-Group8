using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Models.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        // Navigation properties
        public virtual ICollection<Course> Courses { get; set; }
    }
}
