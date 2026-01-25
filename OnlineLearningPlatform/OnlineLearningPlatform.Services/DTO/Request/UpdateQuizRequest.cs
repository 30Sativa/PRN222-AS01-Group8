using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Services.DTO.Request
{
    public class UpdateQuizRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề quiz")]
        [MaxLength(200, ErrorMessage = "Tiêu đề quiz không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng thêm ít nhất một câu hỏi")]
        [MinLength(1, ErrorMessage = "Quiz phải có ít nhất 1 câu hỏi")]
        public List<CreateQuizQuestionRequest> Questions { get; set; } = new List<CreateQuizQuestionRequest>();
    }
}
