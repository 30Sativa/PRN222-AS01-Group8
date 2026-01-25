using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models.Teacher
{
    public class EditSectionViewModel
    {
        public int SectionId { get; set; }
        public Guid CourseId { get; set; }

        [Display(Name = "Tên chương")]
        [Required(ErrorMessage = "Vui lòng nhập tên chương")]
        [MaxLength(200, ErrorMessage = "Tên chương không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Display(Name = "Thứ tự")]
        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự phải là số không âm")]
        public int? OrderIndex { get; set; }
    }
}
