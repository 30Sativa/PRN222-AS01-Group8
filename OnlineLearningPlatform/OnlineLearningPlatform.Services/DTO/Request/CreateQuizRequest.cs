using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Services.DTO.Request
{
    public class CreateQuizRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề quiz")]
        [MaxLength(200, ErrorMessage = "Tiêu đề quiz không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng thêm ít nhất một câu hỏi")]
        [MinLength(1, ErrorMessage = "Quiz phải có ít nhất 1 câu hỏi")]
        public List<CreateQuizQuestionRequest> Questions { get; set; } = new List<CreateQuizQuestionRequest>();
    }

    public class CreateQuizQuestionRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung câu hỏi")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Vui lòng thêm ít nhất 2 đáp án")]
        [MinLength(2, ErrorMessage = "Mỗi câu hỏi phải có ít nhất 2 đáp án")]
        public List<CreateQuizAnswerRequest> Answers { get; set; } = new List<CreateQuizAnswerRequest>();

        [Required(ErrorMessage = "Vui lòng chọn đáp án đúng")]
        [Range(0, int.MaxValue, ErrorMessage = "Đáp án đúng không hợp lệ")]
        public int CorrectAnswerIndex { get; set; }
    }

    public class CreateQuizAnswerRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung đáp án")]
        [MaxLength(200, ErrorMessage = "Đáp án không được vượt quá 200 ký tự")]
        public string UserAnswer { get; set; }
    }
}
