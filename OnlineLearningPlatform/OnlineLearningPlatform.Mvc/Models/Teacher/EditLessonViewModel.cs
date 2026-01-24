using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models.Teacher
{
    public class EditLessonViewModel
    {
        public int LessonId { get; set; }
        public int SectionId { get; set; }

        [Display(Name = "Tên bài học")]
        [Required(ErrorMessage = "Vui lòng nhập tên bài học")]
        [MaxLength(200, ErrorMessage = "Tên bài học không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Display(Name = "Loại bài học")]
        [Required(ErrorMessage = "Vui lòng chọn loại bài học")]
        public string LessonType { get; set; }

        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        [Display(Name = "Thứ tự")]
        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự phải là số không âm")]
        public int? OrderIndex { get; set; }

        public List<SelectListItem> LessonTypes { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "video", Text = "Video" },
            new SelectListItem { Value = "reading", Text = "Tài liệu đọc" },
            new SelectListItem { Value = "quiz", Text = "Bài kiểm tra" },
            new SelectListItem { Value = "assignment", Text = "Bài tập" }
        };
    }
}
