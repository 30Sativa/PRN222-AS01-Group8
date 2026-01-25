using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Services.DTO.Request
{
    public class UpdateLessonRequest
    {
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bài học")]
        [MaxLength(200, ErrorMessage = "Tên bài học không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại bài học")]
        [MaxLength(20)]
        public string LessonType { get; set; } // video | reading | quiz | assignment

        public string Content { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự phải là số không âm")]
        public int? OrderIndex { get; set; }
    }
}
