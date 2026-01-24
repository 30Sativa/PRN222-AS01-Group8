using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Services.DTO.Request
{
    public class CreateSectionRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên chương")]
        [MaxLength(200, ErrorMessage = "Tên chương không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự phải là số không âm")]
        public int? OrderIndex { get; set; }
    }
}
