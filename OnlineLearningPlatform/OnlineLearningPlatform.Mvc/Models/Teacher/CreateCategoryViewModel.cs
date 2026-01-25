using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models.Teacher
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; } = null!;
    }
}
