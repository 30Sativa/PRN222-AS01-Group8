using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models.Teacher
{
    public class CreateCourseViewModel
    {
        [Required(ErrorMessage = "Tiêu đề khóa học là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        [Display(Name = "Tiêu đề khóa học")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Mô tả khóa học là bắt buộc")]
        [Display(Name = "Mô tả khóa học")]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá khóa học (VNĐ)")]
        public decimal Price { get; set; }

        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        // Danh sách categories cho dropdown
        public List<SelectListItem> Categories { get; set; } = new();
    }
}
