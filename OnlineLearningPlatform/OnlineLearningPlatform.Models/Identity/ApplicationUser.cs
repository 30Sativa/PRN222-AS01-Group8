using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
