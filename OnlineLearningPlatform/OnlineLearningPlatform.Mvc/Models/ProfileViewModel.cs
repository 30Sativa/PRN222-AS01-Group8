using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "FullName")]
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        // Statistics
        public int CourseCount { get; set; }
        public int ArticleCount { get; set; }
        public string UserId { get; set; }
    }
}
